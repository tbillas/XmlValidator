using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Schema;

namespace Billas.Xml
{
    public class XmlSchemaProvider : IXmlSchemaProvider
    {
        public string NamespaceUri { get; }
        public Version SchemaVersion { get; }
        public XmlSchemaSet CompiledSchema { get; }
        public string Path { get; }

        public XmlSchemaProvider(string filePath)
        {
            Path = filePath ?? throw new ArgumentNullException(nameof(filePath));

            if(!File.Exists(filePath))
                throw new FileNotFoundException("Schema file is mandatory.", Path);

            CompiledSchema = new XmlSchemaSet {XmlResolver = new XmlUrlResolver()};

            var schema = CompiledSchema.Add(null, filePath);
            NamespaceUri = schema.TargetNamespace;
            if (!string.IsNullOrEmpty(schema.Version) && Version.TryParse(schema.Version, out var version))
                SchemaVersion = version;
            
            CompiledSchema.Compile();
        }

        public override string ToString()
        {
            return $"{NamespaceUri} - {SchemaVersion}";
        }
    }
}