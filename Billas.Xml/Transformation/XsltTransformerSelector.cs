using System;
using System.IO;
using System.Linq;
using System.Xml;

namespace Billas.Xml.Transformation
{
    public class XsltTransformerSelector : IXsltTransformer
    {
        private readonly IXsltTransformer[] _transformers;

        public XsltTransformerSelector(IXsltTransformer[] transformers)
        {
            _transformers = transformers;
        }

        public bool CanHandleXsltVersion(Version version)
        {
            return _transformers.Any(x => x.CanHandleXsltVersion(version));
        }

        private IXsltTransformer FindTransformer(Version xsltVersion)
        {
            var transformer = Array.Find(_transformers, x => x.CanHandleXsltVersion(xsltVersion));
            if (transformer == null)
                throw new ArgumentException($"Cannot find any suitable transfomer for xslt version '{xsltVersion}'");
            return transformer;
        }

        public void Transform(IXsltFileProvider fileProvider, string inputXmlFilePath, string outputFilePath)
        {
            FindTransformer(fileProvider.XsltVersion).Transform(fileProvider, inputXmlFilePath, outputFilePath);
        }

        public void Transform(IXsltFileProvider fileProvider, XmlReader input, XmlWriter output)
        {
            FindTransformer(fileProvider.XsltVersion).Transform(fileProvider, input, output);
        }

        public void Transform(IXsltFileProvider fileProvider, Stream input, Stream output)
        {
            FindTransformer(fileProvider.XsltVersion).Transform(fileProvider, input, output);
        }
        /*
        public static XsltTransformerSelector Load()
        {
            var types = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.DefinedTypes)
                .Where(typeInfo => !typeInfo.IsAbstract && typeof(IXsltTransformer).IsAssignableFrom(typeInfo) && typeInfo != typeof(XsltTransformerSelector))
                .ToArray();

            var instances = new List<IXsltTransformer>();

            foreach (var typeInfo in types)
            {
                if(Activator.CreateInstance(typeInfo) is IXsltTransformer instance)
                    instances.Add(instance);
            }

            return new XsltTransformerSelector(instances.ToArray());
        }
        */
    }
}