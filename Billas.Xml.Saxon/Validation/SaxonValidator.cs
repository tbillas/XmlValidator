using System;
using System.Xml;

namespace Billas.Xml.Validation
{
    public class SaxonValidator : IValidator
    {
        private readonly IXsltFileProvider _xsltFileProvider;
        private readonly SaxonManager _mananger;
        public SaxonValidator(IXsltFileProvider xsltFileProvider)
        {
            _xsltFileProvider = xsltFileProvider;
            _mananger = new SaxonManager();
        }

        public ValidationResult Validate(string xmlFileToValidate)
        {
            var input = new XmlDocument();
            input.Load(xmlFileToValidate);
            var output = new XmlDocument();
            _mananger.Validate(_xsltFileProvider.Path, input, output);

            var result = SchematronValidationResult.Parse(output);


            return new ValidationResult();
        }

        public ValidationResult Validate(XmlReader xmlReaderToValidate)
        {
            throw new NotImplementedException();
        }

        public ValidationResult Validate(XmlDocument xmlDocumentToValidate)
        {
            throw new NotImplementedException();
        }
    }
}