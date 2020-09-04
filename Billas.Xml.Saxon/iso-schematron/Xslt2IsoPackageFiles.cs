namespace Billas.Xml
{
    internal class Xslt2IsoPackageFiles : XsltIsoPackageFiles
    {
        public override string ExtractSchFromXsd => GetPath("ExtractSchFromXSD-2.xsl");
        public override string AbstractExpand => GetPath("iso_abstract_expand.xsl");
        public override string Svrl => GetPath("iso_svrl_for_xslt2.xsl");
        public override string DsdlInclude => GetPath("iso_dsdl_include.xsl");

        public Xslt2IsoPackageFiles(string baseDir)
            : base(baseDir) { }
    }
}