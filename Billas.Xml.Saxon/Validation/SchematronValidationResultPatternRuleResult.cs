using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

namespace Billas.Xml.Validation
{
    public enum SchematronValidationResultPatternRuleResultStatus
    {
        Unknown,
        FailedAssert,
        SuccessfulReport
    }
    public class SchematronValidationResultPatternRuleResult
    {
        public string Test { get; }
        public string Location { get; }
        public string Text { get; }
        public SchematronValidationResultPatternRuleResultStatus Status { get; }

        private SchematronValidationResultPatternRuleResult(string test, string location, string text, SchematronValidationResultPatternRuleResultStatus status)
        {
            Test = test;
            Location = location;
            Text = text;
            Status = status;
        }

        public static SchematronValidationResultPatternRuleResult Parse(XPathNavigator navigator)
        {
            var status = SchematronValidationResultPatternRuleResultStatus.Unknown;
            if (navigator.LocalName == "failed-assert")
                status = SchematronValidationResultPatternRuleResultStatus.FailedAssert;
            else if (navigator.LocalName == "successful-report")
                status = SchematronValidationResultPatternRuleResultStatus.SuccessfulReport;

            var test = navigator.GetAttribute("test", "");
            var location = navigator.GetAttribute("location", "");
            string text = string.Empty;

            if (navigator.HasChildren && navigator.MoveToFirstChild())
            {
                text = navigator.Value.Trim();
                navigator.MoveToParent();
            }

            return new SchematronValidationResultPatternRuleResult(test, location, text, status);
        }
    }

    
    public class SchematronValidationResultPatternRule
    {
        public string Context { get; }
        public string Id { get; }

        private SchematronValidationResultPatternRule(string context, string id)
        {
            Context = context;
            Id = id;
        }

        public IList<SchematronValidationResultPatternRuleResult> Results { get; } = new List<SchematronValidationResultPatternRuleResult>();

        public bool HasFailureResults => Results.Any(result => result.Status == SchematronValidationResultPatternRuleResultStatus.FailedAssert);

        public static SchematronValidationResultPatternRule Parse(XPathNavigator navigator)
        {
            var context = navigator.GetAttribute("context", "");
            var id = navigator.GetAttribute("id", "");
            var rule =  new SchematronValidationResultPatternRule(context, id);

            while (navigator.MoveToNext())
            {
                if (navigator.LocalName == "failed-assert" || navigator.LocalName == "successful-report")
                {
                    var result = SchematronValidationResultPatternRuleResult.Parse(navigator);
                    rule.Results.Add(result);
                }
                else
                {
                    navigator.MoveToPrevious();
                    break;
                }
            }

            return rule;
        }
    }
     
    public class SchematronValidationResultPattern
    {
        public string Document { get; }
        public string Id { get; }
        public string Name { get; }

        private SchematronValidationResultPattern(string document, string id, string name)
        {
            Document = document;
            Id = id;
            Name = name;
        }

        public IList<SchematronValidationResultPatternRule> Rules { get; } = new List<SchematronValidationResultPatternRule>();

        public bool HasFailedRules => Rules.Any(x => x.HasFailureResults);

        public static SchematronValidationResultPattern Parse(XPathNavigator navigator)
        {
            var document = navigator.GetAttribute("document", "");
            var id = navigator.GetAttribute("id", "");
            var name = navigator.GetAttribute("name", "");
            var pattern = new SchematronValidationResultPattern(document, id, name);

            while (navigator.MoveToNext())
            {
                if (navigator.LocalName != "fired-rule")
                {
                    navigator.MoveToPrevious();
                    break;
                }

                var rule = SchematronValidationResultPatternRule.Parse(navigator);
                pattern.Rules.Add(rule);
            }

            return pattern;
        }
    }
   
    public class SchematronValidationResult
    {
        public string NamespaceUri { get; private set; }
        public string NamespacePrefix { get; private set; }

        public IList<SchematronValidationResultPattern> Patterns { get; } = new List<SchematronValidationResultPattern>();

        public bool HasFailedPatterns => Patterns.Any(x => x.HasFailedRules);

        private SchematronValidationResult()
        { }

        public static SchematronValidationResult Parse(XmlDocument schematronResultDocument)
        {
            var navigator = schematronResultDocument.CreateNavigator();
            
            var result = new SchematronValidationResult();

            navigator.MoveToFirstChild();
            var ns = navigator.NamespaceURI;

            if(navigator.LocalName != "schematron-output")
                throw new IndexOutOfRangeException("Not a schematron result file.");

            navigator.MoveToFirstChild();
            while (navigator.NodeType != XPathNodeType.Element)
                navigator.MoveToNext();

            do
            {
                switch (navigator.LocalName)
                {
                    case "ns-prefix-in-attribute-values":
                        result.NamespaceUri = navigator.GetAttribute("uri", ns);
                        result.NamespacePrefix = navigator.GetAttribute("prefix", ns);
                        break;
                    case "active-pattern":
                        result.Patterns.Add(SchematronValidationResultPattern.Parse(navigator));
                        break;
                }
            } while (navigator.MoveToNext());
            
            return result;
        }
    }
}