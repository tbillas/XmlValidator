using System;
using System.IO;

namespace Billas.Xml.Generation
{
    public class SaxonXsltGenerator : IXsltGenerator
    {
        private readonly int _xsltVersion;
        private readonly SaxonManager _manager;

        public SaxonXsltGenerator()
            : this(2) { }

        public SaxonXsltGenerator(int xsltVersion)
        {
            if(xsltVersion != 1 && xsltVersion != 2)
                throw new ArgumentException("Xslt version must either 1 or 2.", nameof(xsltVersion));

            _xsltVersion = xsltVersion;
            _manager = new SaxonManager();
        }

        public void CreateXsltFileFromXsdEmbeddedSchematronRules(string xsdFileWithEmbeddedSchematronRules, string xsltOuputPath, bool overwriteIfExists)
        {
            if (!overwriteIfExists && File.Exists(xsltOuputPath))
            {
                return;
            }

            var isoPackageFiles = XsltIsoPackageFiles.Find(_xsltVersion);

            _manager.CreateXsltFileFromXsdEmbeddedSchematronRules(xsdFileWithEmbeddedSchematronRules, xsltOuputPath, isoPackageFiles);
        }

        public void CreateXsltFileFromSchematronFile(string schFileWithSchematronRules, string xsltOuputPath, bool overwriteIfExists)
        {
            if (!overwriteIfExists && File.Exists(xsltOuputPath))
            {
                return;
            }

            var isoPackageFiles = XsltIsoPackageFiles.Find(_xsltVersion);

            _manager.CreateXsltFileFromSchematronFile(schFileWithSchematronRules, xsltOuputPath, isoPackageFiles);
        }
    }
}
