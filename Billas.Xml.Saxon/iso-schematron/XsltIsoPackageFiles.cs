using System;
using System.IO;
using System.Reflection;

namespace Billas.Xml
{
    internal abstract class XsltIsoPackageFiles : IIsoPackageFiles
    {
        private readonly string _baseDir;
        public abstract string ExtractSchFromXsd { get; }
        public abstract string AbstractExpand { get; }
        public abstract string Svrl { get; }
        public abstract string DsdlInclude { get; }

        protected XsltIsoPackageFiles(string baseDir)
        {
            _baseDir = baseDir ?? throw new ArgumentNullException(nameof(baseDir));
            if (!Directory.Exists(_baseDir))
                throw new DirectoryNotFoundException($"Directory {baseDir} not found.");
        }

        protected string GetPath(string fileName)
        {
            var path = Path.Combine(_baseDir, fileName);
            if (!File.Exists(path))
                throw new FileNotFoundException($"File '{path}' was not found.", path);

            return path;
        }

        public static IIsoPackageFiles Find(int version)
        {
            var uri = new Uri(Assembly.GetEntryAssembly().Location);
            var dir = Path.GetDirectoryName(uri.AbsolutePath);
            var isoSchematron = Path.Combine(dir, "iso-schematron");
            if (!Directory.Exists(isoSchematron))
                throw new DirectoryNotFoundException($"Could not find directory {isoSchematron}");

            var versioned = Path.Combine(isoSchematron, $"iso-schematron-xslt{version}");
            if (!Directory.Exists(versioned))
                throw new DirectoryNotFoundException($"Could not find directory {versioned}");

            return version == 2 ? (IIsoPackageFiles) new Xslt2IsoPackageFiles(versioned) : new Xslt1IsoPackageFiles(versioned);
        }
    }
}