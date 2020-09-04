using System;
using System.IO;
using System.Xml;

namespace Billas.Xml.Transformation
{
    public class SaxonTransformer : IXsltTransformer
    {
        private readonly SaxonManager _mananger;
        public SaxonTransformer()
        {
            _mananger = new SaxonManager();
        }

        public bool CanHandleXsltVersion(Version xsltVersion)
        {
            return xsltVersion.Major <= 3;
        }

        public void Transform(IXsltFileProvider fileProvider, string inputXmlFilePath, string outputFilePath)
        {
            _mananger.Transform(inputXmlFilePath, fileProvider.Path, outputFilePath);
        }

        public void Transform(IXsltFileProvider fileProvider, XmlReader input, XmlWriter output)
        {
            _mananger.Transform(input, fileProvider.Path, output);
        }

        public void Transform(IXsltFileProvider fileProvider, Stream input, Stream output)
        {
            _mananger.Transform(input, fileProvider.Path, output);
        }
    }
}