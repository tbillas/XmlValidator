using System.Collections.Generic;
using System.Xml.Schema;

namespace Billas.Xml.Validation
{
    public class ValidationResult
    {
        private readonly List<string> _warnings = new List<string>();
        private readonly List<string> _errors = new List<string>();

        public int ErrorCount => _errors.Count;
        public int WarningCount => _warnings.Count;

        public IEnumerable<string> Errors => _errors;
        public IEnumerable<string> Warnings => _warnings;

        public ValidationResult Add(ValidationEventArgs args)
        {
            switch (args.Severity)
            {
                case XmlSeverityType.Error:
                    _errors.Add(args.Message);
                    break;
                case XmlSeverityType.Warning:
                    _warnings.Add(args.Message);
                    break;
            }

            return this;
        }

        public override string ToString()
        {
            return $"{ErrorCount} errors, {WarningCount} warnings.";
        }
    }
}