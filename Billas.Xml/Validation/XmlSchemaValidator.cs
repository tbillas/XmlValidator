using System.Xml;
using System.Xml.Schema;

namespace Billas.Xml.Validation
{
    public class XmlSchemaValidator : IValidator
    {
        private readonly IXmlSchemaProvider _xmlSchemaProvider;

        public XmlSchemaValidator(IXmlSchemaProvider xmlSchemaProvider)
        {
            _xmlSchemaProvider = xmlSchemaProvider;
        }

        public ValidationResult Validate(string xmlFileToValidate)
        {
            var result = new ValidationResult();

            var settings = new XmlReaderSettings
            {
                Schemas = _xmlSchemaProvider.CompiledSchema,
                ValidationType = ValidationType.Schema
            };

            settings.ValidationEventHandler += (sender, args) => result.Add(args);

            using (var xmlReader = XmlReader.Create(xmlFileToValidate, settings))
            {
                while (xmlReader.Read()) { }
            }

            return result;
        }

        public ValidationResult Validate(XmlReader xmlReaderToValidate)
        {
            var xmlDocumentToValidate = new XmlDocument();
            xmlDocumentToValidate.Load(xmlReaderToValidate);
            return Validate(xmlDocumentToValidate);
        }

        public ValidationResult Validate(XmlDocument xmlDocumentToValidate)
        {
            var result = new ValidationResult();
            xmlDocumentToValidate.Schemas = _xmlSchemaProvider.CompiledSchema;
            xmlDocumentToValidate.Validate((sender, args) => result.Add(args));
            return result;
        }
    }

    public class XmlValidator : IValidator
    {
        public ValidationResult Validate(string xmlFileToValidate)
        {
            var result = new ValidationResult();

            var settings = new XmlReaderSettings
            {
                ValidationType = ValidationType.Schema,
                ValidationFlags = XmlSchemaValidationFlags.ProcessInlineSchema | XmlSchemaValidationFlags.ProcessSchemaLocation | XmlSchemaValidationFlags.ReportValidationWarnings
            };

            settings.ValidationEventHandler += (sender, args) => result.Add(args);

            using (var xmlReader = XmlReader.Create(xmlFileToValidate, settings))
            {
                while (xmlReader.Read()) { }
            }

            return result;
        }

        public ValidationResult Validate(XmlReader xmlReaderToValidate)
        {
            throw new System.NotImplementedException();
        }

        public ValidationResult Validate(XmlDocument xmlDocumentToValidate)
        {
            
            throw new System.NotImplementedException();
        }
    }
}