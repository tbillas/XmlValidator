using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Xml;
using System.Xml.Xsl;
using Saxon.Api;

namespace Billas.Xml
{
    internal class SaxonManager : IDisposable
    {
        private readonly Lazy<Processor> _processor;
        private readonly Lazy<XsltCompiler> _xsltCompiler;
        private readonly Lazy<DocumentBuilder> _documentBuilder;
        private readonly ConcurrentDictionary<string, XsltExecutable> _loadedTransformers = new ConcurrentDictionary<string, XsltExecutable>();

        public SaxonManager()
        {
            _processor = new Lazy<Processor>(() => new Processor(), LazyThreadSafetyMode.ExecutionAndPublication);
            _xsltCompiler = new Lazy<XsltCompiler>(() => _processor.Value.NewXsltCompiler(), LazyThreadSafetyMode.ExecutionAndPublication);
            _documentBuilder = new Lazy<DocumentBuilder>(() => _processor.Value.NewDocumentBuilder(), LazyThreadSafetyMode.ExecutionAndPublication);
        }

        private void Transform(XdmNode input, string xsltPath, XmlDestination output)
        {
            var xsltExecutable = _loadedTransformers.GetOrAdd(xsltPath, path =>
            {
                var errors = new List<StaticError>();
                try
                {
                    var compiler = _xsltCompiler.Value;
                    compiler.ErrorList = errors;
                    var compiled = compiler.Compile(new Uri(path));
                    return compiled;
                }
                catch (Exception)
                {
                    if (errors.Count == 0)
                    {
                        throw;
                    }
                    var exception = new XsltException(errors[0].ToString());
                    for (var i = 1; i < errors.Count; i++)
                    {
                        exception = new XsltException(errors[i].ToString(), exception);
                    }
                    throw exception;
                }
            });

            var xsltTransformer = xsltExecutable.Load();
            xsltTransformer.InitialContextNode = input;
            xsltTransformer.Run(output);
        }

        public void Transform(string input, string xsltFilePath, string output)
        {
            var destination = new TextWriterDestination(XmlWriter.Create(output));
            Transform(_documentBuilder.Value.Build(XmlReader.Create(input)), xsltFilePath, destination);
        }
        public void Transform(XmlReader input, string xsltFilePath, XmlWriter output)
        {
            var destination = new TextWriterDestination(XmlWriter.Create(output));
            Transform(_documentBuilder.Value.Build(input), xsltFilePath, destination);
        }
        public void Transform(Stream input, string xsltFilePath, Stream output)
        {
            var destination = new TextWriterDestination(XmlWriter.Create(output)) {CloseAfterUse = true};
            Transform(_documentBuilder.Value.Build(input), xsltFilePath, destination);
        }


        public void CreateXsltFileFromXsdEmbeddedSchematronRules(string xsdFileWithEmbeddedSchematronRules, string xsltOuputPath, IIsoPackageFiles isoPackageFiles)
        {
            if (!File.Exists(xsdFileWithEmbeddedSchematronRules))
                throw new FileNotFoundException($"Xsd file with embedded schematron rules was not found ({xsdFileWithEmbeddedSchematronRules}).", xsdFileWithEmbeddedSchematronRules);

            using (var xmlReader = XmlReader.Create(xsdFileWithEmbeddedSchematronRules))
            {
                var source = _documentBuilder.Value.Build(xmlReader);

                var extraktSchFromXsdTransformer = _xsltCompiler.Value.Compile(new Uri(isoPackageFiles.ExtractSchFromXsd)).Load();
                extraktSchFromXsdTransformer.InitialContextNode = source;

                var isoAbstractExpandTransformer = _xsltCompiler.Value.Compile(new Uri(isoPackageFiles.AbstractExpand)).Load();

                var isoSvrlForXsltTransformer = _xsltCompiler.Value.Compile(new Uri(isoPackageFiles.Svrl)).Load();
                isoAbstractExpandTransformer.Destination = isoSvrlForXsltTransformer;

                using (var xmlWriter = XmlWriter.Create(xsltOuputPath, new XmlWriterSettings { Indent = true }))
                {
                    isoSvrlForXsltTransformer.Destination = new TextWriterDestination(xmlWriter);
                    extraktSchFromXsdTransformer.Run(isoAbstractExpandTransformer);
                }
            }
        }
        public void CreateXsltFileFromSchematronFile(string schFileWithSchematronRules, string xsltOuputPath, IIsoPackageFiles isoPackageFiles)
        {
            if (!File.Exists(schFileWithSchematronRules))
                throw new FileNotFoundException($"Sch file with schematron rules was not found ({schFileWithSchematronRules}).", schFileWithSchematronRules);

            var source = _documentBuilder.Value.Build(XmlReader.Create(schFileWithSchematronRules));

            var isoDsdlIncludeTransformer = _xsltCompiler.Value.Compile(new Uri(isoPackageFiles.DsdlInclude)).Load();
            isoDsdlIncludeTransformer.InitialContextNode = source;

            var isoAbstractExpandTransformer = _xsltCompiler.Value.Compile(new Uri(isoPackageFiles.AbstractExpand)).Load();


            var isoSvrlForXsltTransformer = _xsltCompiler.Value.Compile(new Uri(isoPackageFiles.Svrl)).Load();
            isoAbstractExpandTransformer.Destination = isoSvrlForXsltTransformer;
            isoSvrlForXsltTransformer.Destination = new TextWriterDestination(XmlWriter.Create(xsltOuputPath, new XmlWriterSettings { Indent = true }));

            isoDsdlIncludeTransformer.Run(isoAbstractExpandTransformer);
        }


        public void Validate(string validationXsltPath, string xmlFileToValidate, string resultFile)
        {
            var input = _documentBuilder.Value.Build(new Uri(xmlFileToValidate));
            Transform(input, validationXsltPath, new TextWriterDestination(XmlWriter.Create(resultFile)));
        }

        public void Validate(string validationXsltPath, XmlReader xmlReaderToValidate, XmlWriter resutWriter)
        {
            var input = _documentBuilder.Value.Build(xmlReaderToValidate);
            Transform(input, validationXsltPath, new TextWriterDestination(resutWriter));
        }

        public void Validate(string validationXsltPath, XmlDocument xmlDocumentToValidate, XmlDocument resultDocument)
        {
            var input = _documentBuilder.Value.Build(xmlDocumentToValidate);
            
            using (var stream = new MemoryStream())
            {
                var destination = new TextWriterDestination(XmlWriter.Create(stream)) { CloseAfterUse = true };
                Transform(input, validationXsltPath, destination);

                stream.Position = 0;
                resultDocument.Load(stream);
            }
        }

        private bool _disposed = false;
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing && !_disposed)
            {
                _loadedTransformers.Clear();
                _disposed = true;
            }
        }
    }
}
