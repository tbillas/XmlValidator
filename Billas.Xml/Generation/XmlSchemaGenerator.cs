using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

namespace Billas.Xml.Generation
{
    public class XmlSchemaGenerator : IXmlSchemaGenerator
    {
        public void GenerateSchemaFromXml(string inputXmlFile, string outputXsdFile, bool overwriteIfExists = false)
        {
            XmlSchemaSet schemaSet;
            using (var reader = XmlReader.Create(inputXmlFile))
            {
                schemaSet = new XmlSchemaInference().InferSchema(reader);
            }

            var mode = overwriteIfExists ? FileMode.Create : FileMode.CreateNew;

            foreach (XmlSchema schema in schemaSet.Schemas())
            {
                using (var file = new FileStream(outputXsdFile, mode, FileAccess.Write))
                using (var writer = XmlWriter.Create(file, new XmlWriterSettings{Indent = true, Encoding = Encoding.UTF8}))
                {
                    schema.Write(writer);
                }
            }
        }

        public void GenerateSchemaFromXml(XmlReader inputXmlReader, XmlWriter outputXsdWriter)
        {
            var schemaSet = new XmlSchemaInference().InferSchema(inputXmlReader);
            if(schemaSet.Count > 1) throw new Exception("Cannot write multiple schemas to single XmlWriter");

            foreach (XmlSchema schema in schemaSet.Schemas())
            {
                schema.Write(outputXsdWriter);
            }
        }
    }
}