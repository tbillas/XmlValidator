using System.Xml;

namespace Billas.Xml.Validation
{
    public class SchematronValidator : IValidator
    {
        public ValidationResult Validate(string xmlFileToValidate)
        {
            throw new System.NotImplementedException();
        }

        public ValidationResult Validate(XmlReader xmlToValidate)
        {
            throw new System.NotImplementedException();
        }

        public ValidationResult Validate(XmlDocument xmlDocumentToValidate)
        {
            throw new System.NotImplementedException();
        }
    }
}