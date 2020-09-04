using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;

namespace Billas.Xml
{
    public interface IXmlFileInformationResolver
    {
        XmlFileInformation GetInformation(string filePath);
    }

    public class XmlFileInformationResolver : IXmlFileInformationResolver
    {
        public XmlFileInformation GetInformation(string filePath)
        {
            if (filePath == null) throw new ArgumentNullException(nameof(filePath));
            if(!File.Exists(filePath)) throw new ArgumentException("File does not exist.", nameof(filePath));

            using (var reader = XmlReader.Create(filePath))
            {
                if (reader.NodeType == XmlNodeType.None)
                    reader.Read();

                string version = null;

                if (reader.NodeType == XmlNodeType.XmlDeclaration)
                {
                    version = reader.GetAttribute("version");
                }
                
                reader.MoveToContent();

                var ns = reader.Prefix == "xs" && reader.LocalName == "schema" ? reader.GetAttribute("targetNamespace") : reader.LookupNamespace("");

                return new XmlFileInformation { NamespaceUri = ns, Version = version != null ? Version.Parse(version) : new Version(), Path = filePath };
            }
        }
    }

    public class XmlFileInformation : IEquatable<XmlFileInformation>
    {
        public string NamespaceUri { get; set; }
        public Version Version { get; set; }

        public string Path { get; set; }

        public bool Equals(XmlFileInformation other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return string.Equals(NamespaceUri, other.NamespaceUri) && Equals(Version, other.Version);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((XmlFileInformation)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((NamespaceUri?.GetHashCode() ?? 0) * 397) ^ (Version != null ? Version.GetHashCode() : 0);
            }
        }

        public static bool operator ==(XmlFileInformation left, XmlFileInformation right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(XmlFileInformation left, XmlFileInformation right)
        {
            return !Equals(left, right);
        }
    }
}
