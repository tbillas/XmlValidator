using System;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace Billas.Xml.Transformation
{
    /// <summary>
    /// Only supports the XSLT 1.0 syntax
    /// </summary>
    public class XslCompiledTransformer : IXsltTransformer
    {
        public bool CanHandleXsltVersion(Version xsltVersion) => xsltVersion.Major <= 1;

        public void Transform(IXsltFileProvider fileProvider, string inputXmlFilePath, string outputFilePath)
        {
            if(!CanHandleXsltVersion(fileProvider.XsltVersion))
                throw new Exception($"This transformer can handle xslt version 1. Supplied xslt is {fileProvider.XsltVersion}");

            var transform = new XslCompiledTransform();
            transform.Load(fileProvider.Path);
            transform.Transform(inputXmlFilePath, outputFilePath);
        }

        public void Transform(IXsltFileProvider fileProvider, XmlReader input, XmlWriter output)
        {
            if (!CanHandleXsltVersion(fileProvider.XsltVersion))
                throw new Exception($"This transformer kan handle xslt version 1. Supplied xslt is {fileProvider.XsltVersion}");

            var transform = new XslCompiledTransform();
            transform.Load(fileProvider.Path);
            transform.Transform(input, output);
        }

        public void Transform(IXsltFileProvider fileProvider, Stream input, Stream output)
        {
            if (!CanHandleXsltVersion(fileProvider.XsltVersion))
                throw new Exception($"This transformer kan handle xslt version 1. Supplied xslt is {fileProvider.XsltVersion}");

            var transform = new XslCompiledTransform();
            transform.Load(fileProvider.Path);
            using (var reader = XmlReader.Create(input))
            {
                transform.Transform(reader, null, output);
            }
        }

        
    }
}