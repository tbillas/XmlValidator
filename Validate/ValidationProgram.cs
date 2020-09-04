using System;
using System.Xml;
using Billas.Xml;
using Billas.Xml.Validation;
using PowerArgs;
using XmlSchemaException = System.Xml.Schema.XmlSchemaException;

namespace Validate
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class ValidationProgram
    {
        [HelpHook, ArgShortcut("-?"), ArgDescription("Shows this help")]
        public bool Help { get; set; }

        [ArgActionMethod, ArgDescription("Validate xml file")]
        public void Xml(
            [ArgRequired, ArgExistingFile, ArgDescription("Full path to xml file to validate.")] string file,
            [ArgExistingFile, ArgDescription("If schema is given, that schema file is used. Otherwise an inline (schemaLocation) is searched.")] string schema,
            [ArgExistingFile, ArgDescription("If schematron is given, that xslt file is used to do schematron validation.")] string schematron
        )
        {
            
            XmlSchemaProvider xmlSchemaProvider = null;
            if (!string.IsNullOrEmpty(schema))
            {
                Console.Write("Using ");
                xmlSchemaProvider = Schema(schema);
            }

#if NET461
            XsltFileProvider xsltFileProvider = null;
            if (!string.IsNullOrEmpty(schematron))
            {
                Console.Write("Using: ");
                xsltFileProvider = Schematron(schematron);
            }
#endif

            Console.Write($"Validating xml:\t{file}  ");

            var validateionResult = xmlSchemaProvider != null
                ? new XmlSchemaValidator(xmlSchemaProvider).Validate(file)
                : new XmlValidator().Validate(file);

            if (validateionResult.WarningCount > 0 || validateionResult.ErrorCount > 0)
            {
                Console.WriteLine();
                foreach (var warning in validateionResult.Warnings)
                {
                    Console.Write("-> ");
                    ColorConsole.Warning(warning);
                }

                foreach (var error in validateionResult.Errors)
                {
                    Console.Write("-> ");
                    ColorConsole.Error(error);
                }
            }
            else
            {
#if NET461
                if (xsltFileProvider != null)
                {
                    var saxonResult = new SaxonValidator(xsltFileProvider).Validate(file);
                }
#endif
                ColorConsole.Ok();
            }
        }

        [ArgActionMethod, ArgShortcut("xsd"), ArgDescription("Validate schema (.xsd) file.")]
        public XmlSchemaProvider Schema(
            [ArgRequired, ArgExistingFile, ArgDescription("Full path to xsd file to validate.")] string schema)
        {
            try
            {
                var schemaProvider = new XmlSchemaProvider(schema);
                Console.Write("Schema:\t{0} [v{1}] ({2})  ", schemaProvider.NamespaceUri, schemaProvider.SchemaVersion, schemaProvider.Path);
                ColorConsole.Ok();
                return schemaProvider;
            }
            catch (XmlSchemaException e)
            {
                Console.WriteLine("File: {0}", schema);
                Console.Write("-> ");
                ColorConsole.Error(e.Message);
                Console.WriteLine($"-> At line {e.LineNumber}, pos {e.LinePosition}");
                return null;
            }
            catch (XmlException e)
            {
                Console.WriteLine("File: {0}", schema);
                Console.Write("-> ");
                ColorConsole.Error(e.Message);
                return null;
            }
        }

        [ArgActionMethod, ArgShortcut("xslt"), ArgDescription("Validate schematron (.xslt) file.")]
        public XsltFileProvider Schematron(
            [ArgRequired, ArgExistingFile, ArgDescription("Full path to xslt file to validate")] string schematron)
        {
            var xsltFileProvider = new XsltFileProvider(schematron);
            Console.Write("Schmatron: {0} [xsltVersion: {1}]", xsltFileProvider.Path, xsltFileProvider.XsltVersion);
            ColorConsole.Ok();
            return xsltFileProvider;
        }
    }
}
