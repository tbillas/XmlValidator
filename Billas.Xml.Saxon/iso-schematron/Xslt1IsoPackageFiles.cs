namespace Billas.Xml
{
    internal class Xslt1IsoPackageFiles : XsltIsoPackageFiles
    {
        public override string ExtractSchFromXsd => GetPath("ExtractSchFromXSD.xsl");
        public override string AbstractExpand => GetPath("iso_abstract_expand.xsl");
        public override string Svrl => GetPath("iso_svrl_for_xslt1.xsl");
        public override string DsdlInclude => GetPath("iso_dsdl_include.xsl");

        public Xslt1IsoPackageFiles(string baseDir)
            : base(baseDir) { }
    }
}