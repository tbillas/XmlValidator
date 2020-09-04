using System;

namespace Billas.Xml
{
    public interface IXsltFileProvider
    {
        Version XsltVersion { get; }
        string Path { get; }
    }
}