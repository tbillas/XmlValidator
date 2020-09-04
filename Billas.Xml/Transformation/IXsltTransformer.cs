using System;
using System.IO;
using System.Xml;

namespace Billas.Xml.Transformation
{
    /// <summary>
    /// Transforms XML data using an XSLT style sheet.
    /// Input must be a xml file. Output could be anything (xml, html ...)
    /// </summary>
    public interface IXsltTransformer
    {
        bool CanHandleXsltVersion(Version xsltVersion);

        void Transform(IXsltFileProvider fileProvider, string inputXmlFilePath, string outputFilePath);
        void Transform(IXsltFileProvider fileProvider, XmlReader input, XmlWriter output);
        void Transform(IXsltFileProvider fileProvider, Stream input, Stream output);
    }
}
