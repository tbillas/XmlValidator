using System;
using System.Collections.Generic;
using System.IO;
using Billas.Xml;
using Billas.Xml.Generation;
using Billas.Xml.Transformation;
using PowerArgs;

namespace Generate
{
    [ArgExceptionBehavior(ArgExceptionPolicy.StandardExceptionHandling)]
    public class GenerationProgram
    {
        [HelpHook, ArgShortcut("-?"), ArgDescription("Shows this help")]
        public bool Help { get; set; }

        [ArgActionMethod, ArgDescription("Normalize (merge/concatenate) .xsd (schema) file that includes other .xsd files into one single file.")]
#if NETFRAMEWORK
        [ArgExample("Generate.exe merge C:/Path/to/file.xsd [-output C:/Path/to/output.xsd]", "Merge file")]
#elif NETCOREAPP
        [ArgExample("dotnet Generate.dll merge C:/Path/to/file.xsd [-output C:/Path/to/output.xsd]", "Merge file")]
#endif
        public void Merge(
            [ArgRequired, ArgExistingFile, ArgDescription("Full path to .xsd (schema) file that includes other .xsd files.")] string schema,
            [ArgDescription("Full path to output .xsd file (optional)")] string output,
            [ArgDescription("Version of the new schema file (optional)")] string version)
        {
            var settings = new XmlSchemaMergeSettings(version) {OverwriteExisting = true};
            new XmlSchemaMerge().Merge(schema, output, settings);
        }

        [ArgActionMethod, ArgDescription("Create schema (xsd) from a xml-file.")]
        public void Schema(
            [ArgRequired, ArgExistingFile, ArgDescription("Full path to .xml file (mandatory)")] string xml,
            [ArgDescription("Full path to output .xsd file (optional)")] string output)
        {
            new XmlSchemaGenerator().GenerateSchemaFromXml(xml, output ?? CreateOuputFilePathFromInputFilePath(xml, "xsd", ""));
        }

        [ArgActionMethod, ArgDescription("Create styled output from a .xml file using a stylesheet (xslt).")]
        public void Style(
            [ArgRequired, ArgExistingFile, ArgDescription("Full path to .xml file (mandatory)")] string xml,
            [ArgRequired, ArgExistingFile, ArgDescription("Full path to .xslt file (stylesheet) (mandatory)")] string stylesheet,
            [ArgDescription("Full path to output file (optional)")] string output)
        {
            var transformers = new List<IXsltTransformer> {new XslCompiledTransformer()};
#if SAXONSUPPORT
            transformers.Add(new SaxonTransformer());
#endif
            IXsltTransformer xsltTransformer = new XsltTransformerSelector(transformers.ToArray());
            XsltFileProvider xsltFileProvider = null;
            try
            {
                xsltFileProvider = new XsltFileProvider(stylesheet);
            }
            catch (Exception e)
            {
                Console.WriteLine($"When loading stylesheet {stylesheet} -> {e.Message}");
                Environment.Exit(1);
            }

            try
            {
                xsltTransformer.Transform(xsltFileProvider, xml, output ?? CreateOuputFilePathFromInputFilePath(xml, "html", ""));
            }
            catch (Exception e)
            {
                Console.WriteLine($"When styling {xml} -> {e.Message}");
                Environment.Exit(1);
            }
        }

        [ArgActionMethod, ArgDescription("Create styled output from a directory of .xml files using a stylesheet (xslt).")]
        public void StyleAll(
            [ArgRequired, ArgExistingDirectory, ArgDescription("Full path to directory containing xml files (mandatory)")] string dir,
            [ArgRequired, ArgExistingFile, ArgDescription("Full path to .xslt file (stylesheet) (mandatory)")] string stylesheet,
            [ArgExistingDirectory, ArgDescription("Full path to output directory (optional)")] string output)
        {
            var transformers = new List<IXsltTransformer> { new XslCompiledTransformer() };
#if SAXONSUPPORT
            transformers.Add(new SaxonTransformer());
#endif
            IXsltTransformer xsltTransformer = new XsltTransformerSelector(transformers.ToArray());
            XsltFileProvider xsltFileProvider = null;
            try
            {
                xsltFileProvider = new XsltFileProvider(stylesheet);
            }
            catch (Exception e)
            {
                Console.WriteLine($"When loading stylesheet {stylesheet} -> {e.Message}");
                Environment.Exit(1);
            }

            if (string.IsNullOrEmpty(output))
                output = dir;

            foreach (var xml in Directory.EnumerateFiles(dir, "*.xml"))
            {
                try
                {
                    var outFile = Path.Combine(output, $"{Path.GetFileNameWithoutExtension(xml)}.html");
                    xsltTransformer.Transform(xsltFileProvider, xml, outFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine($"When styling {xml} -> {e.Message}");
                    Environment.Exit(1);
                }
            }
        }

#if NET461
        [ArgActionMethod, ArgDescription("Create schematron file (xslt) from schematron rules. Rules can be read in a .sch (schematron) file, or inlined insied a .xsd (xml schema) file.")]
        public void Schematron(
            [ArgRequired, ArgExistingFile, ArgDescription("Full path to file containing schematron rules. Can be .sch-file or .xsd with inline rules.")] string file,
            [ArgDescription("Xslt version to use. 1 or 2. Default is 2."), ArgShortcut("-v")] int? version,
            [ArgDescription("Full path to output file (optional)")] string output)
        {
            var xsltGenerator = new SaxonXsltGenerator(version ?? 2);
            var outputFile = output ?? CreateOuputFilePathFromInputFilePath(file, "xslt", "_schematron");
            var info = new FileInfo(file);
            if (info.Extension == ".xsd")
                xsltGenerator.CreateXsltFileFromXsdEmbeddedSchematronRules(file, outputFile, true);
            else if (info.Extension == ".sch")
                xsltGenerator.CreateXsltFileFromSchematronFile(file, outputFile, true);
            else
                throw new Exception($"Unknown file extension ({info.Extension}) of input file.");
        }
#endif
        private static string CreateOuputFilePathFromInputFilePath(string inputFilePath, string outputExtension, string nameExt = "_generated")
        {
            return $"{Path.GetDirectoryName(inputFilePath)}{Path.DirectorySeparatorChar}{Path.GetFileNameWithoutExtension(inputFilePath)}{nameExt}.{outputExtension}";
        }
    }
}
