using System;
using System.IO;
using System.Xml.Linq;

namespace Billas.Xml
{
    public class XsltFileProvider : IXsltFileProvider
    {
        public Version XsltVersion { get; }
        public string Path { get; }

        public XsltFileProvider(string filePath)
        {
            Path = filePath ?? throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Xslt file is mandatory.", Path);

            var versionAttribute = XDocument.Load(filePath).Root?.Attribute("version");
            if (versionAttribute != null && !string.IsNullOrEmpty(versionAttribute.Value))
            {
                if(!Version.TryParse(versionAttribute.Value, out var version))
                    throw new Exception($"Version string '{versionAttribute}' is not in a correct version format.");
                XsltVersion = version;
            }
            else
                throw new Exception("Could not find any version information in file. Please use ctor(string, Version) to apply version information.");
        }

        public XsltFileProvider(string filePath, Version xsltVersion)
        {
            Path = filePath ?? throw new ArgumentNullException(nameof(filePath));

            if (!File.Exists(filePath))
                throw new FileNotFoundException("Xslt file is mandatory.", Path);

            XsltVersion = xsltVersion;
        }

        public XsltFileProvider(string filePath, int majorXsltVersion)
            : this(filePath, new Version(majorXsltVersion, 0)) { }
    }
}