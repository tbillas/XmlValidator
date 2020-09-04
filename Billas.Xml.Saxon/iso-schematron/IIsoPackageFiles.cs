namespace Billas.Xml
{
    internal interface IIsoPackageFiles
    {
        string ExtractSchFromXsd { get; }
        string AbstractExpand { get; }
        string Svrl { get; }
        string DsdlInclude { get; }
    }
}