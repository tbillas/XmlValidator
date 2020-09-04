using System.Xml;

namespace Billas.Xml.Generation
{
    public interface IXmlSchemaGenerator
    {
        void GenerateSchemaFromXml(string inputXmlFile, string outputXsdFile, bool overwriteIfExists = false);
        void GenerateSchemaFromXml(XmlReader inputXmlReader, XmlWriter outputXsdWriter);
    }
}