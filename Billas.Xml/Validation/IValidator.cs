using System.Xml;

namespace Billas.Xml.Validation
{
    public interface IValidator
    {
        ValidationResult Validate(string xmlFileToValidate);
        ValidationResult Validate(XmlReader xmlReaderToValidate);
        ValidationResult Validate(XmlDocument xmlDocumentToValidate);
    }
}
