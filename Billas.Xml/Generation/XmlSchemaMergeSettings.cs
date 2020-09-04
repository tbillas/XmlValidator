using System;

namespace Billas.Xml.Generation
{
    public class XmlSchemaMergeSettings
    {
        public Version SetSchemaVersion { get; set; }
        public bool IncludeOnlyUsedTypes { get; set; }
        public bool OverwriteExisting { get; set; }

        public XmlSchemaMergeSettings()
        {
            IncludeOnlyUsedTypes = true;
            OverwriteExisting = false;
        }

        public XmlSchemaMergeSettings(string version)
            : this()
        {
            if (!string.IsNullOrEmpty(version) && Version.TryParse(version, out var v))
                SetSchemaVersion = v;
        }

        public static XmlSchemaMergeSettings Default => new XmlSchemaMergeSettings();
    }
}