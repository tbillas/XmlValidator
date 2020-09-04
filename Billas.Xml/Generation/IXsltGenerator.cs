namespace Billas.Xml.Generation
{
    public interface IXsltGenerator
    {
        void CreateXsltFileFromXsdEmbeddedSchematronRules(string xsdFileWithEmbeddedSchematronRules, string xsltOuputPath, bool overwriteIfExists);
        void CreateXsltFileFromSchematronFile(string schFileWithSchematronRules, string xsltOuputPath, bool overwriteIfExists);
    }
}