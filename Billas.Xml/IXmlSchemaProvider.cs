using System;
using System.Xml.Schema;

namespace Billas.Xml
{
    public interface IXmlSchemaProvider
    {
        string NamespaceUri { get; }

        Version SchemaVersion { get; }

        XmlSchemaSet CompiledSchema { get; }

        string Path { get; }
    }
}