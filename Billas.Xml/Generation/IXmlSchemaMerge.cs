using System;
using System.IO;
using System.Xml.Schema;

namespace Billas.Xml.Generation
{
    /// <summary>
    /// Use this to merge multiple .xsd files that is used to form one xml schema into a single .xsd file.
    /// </summary>
    public interface IXmlSchemaMerge
    {
        /// <summary>
        /// Create a single .xsd file from a collection of linked/included schema files. Just supply the "main" schema file.
        /// </summary>
        /// <param name="xsdFileWithOtherIncludedOrLinkedXsdFiles">The "main" schema file (the starting point)</param>
        /// <param name="outputFilePath">Full path to new output file. If null one is generated.</param>
        /// <param name="settings">Configuration for merging.</param>
        void Merge(string xsdFileWithOtherIncludedOrLinkedXsdFiles, string outputFilePath, XmlSchemaMergeSettings settings = null);

        /// <summary>
        /// Create a single XmlSchema from a collection of linked/included schema files.
        /// </summary>
        /// <param name="schemaSet">SchemaSet containing just 1 loaded schema (but that includes other schema files)</param>
        /// <param name="outStream">Stream to write to. Mandatory.</param>
        /// <param name="settings">Configuration for merging.</param>
        void Merge(XmlSchemaSet schemaSet, Stream outStream, XmlSchemaMergeSettings settings = null);
    }
}