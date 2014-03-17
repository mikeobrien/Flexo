using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;
using Flexo.Extensions;

namespace Flexo
{
    public class XmlJsonEncoder : IJsonEncoder
    {
        public Stream Encode(JElement jsonElement, Encoding encoding = null, bool pretty = false)
        {
            try
            {
                var xmlElement = new XElement(XmlJson.RootElementName);
                Save(jsonElement, xmlElement);
                var stream = new MemoryStream();
                // Whitespace is only supported in >= 4.5:
                // using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, encoding, false, pretty))
                using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, encoding ?? Encoding.UTF8, false))
                {
                    xmlElement.Save(writer);
                    writer.Flush();
                    stream.Position = 0;
                    return stream;
                }
            }
            catch (Exception exception)
            {
                throw new JsonEncodeException(exception);
            }
        }

        private static void Save(JElement jsonElement, XElement xmlElement)
        {
            SetElementType(xmlElement, jsonElement.Type);
            if (jsonElement.IsValue) SetElementValue(jsonElement, xmlElement);
            else jsonElement.ForEach(x => Save(x, CreateElement(jsonElement.IsArray, x, xmlElement)));
        }

        private static XElement CreateElement(bool isArrayItem, JElement jsonElement, XElement xmlElement)
        {
            var name = isArrayItem ? XmlJson.ArrayItemElementName : jsonElement.Name;
            if (isArrayItem || name.IsValidXmlName()) return xmlElement.CreateElement(name);
            XNamespace @namespace = XmlJson.ArrayItemElementName;
            var child = new XElement(
                @namespace + XmlJson.ArrayItemElementName,
                new XAttribute(XNamespace.Xmlns + "a", XmlJson.ArrayItemElementName),
                new XAttribute(XmlJson.ArrayItemElementName, name));
            xmlElement.Add(child);
            return child;
        }

        private static void SetElementValue(JElement jsonElement, XElement xmlElement)
        {
            switch (jsonElement.Type)
            {
                case ElementType.Null: return;
                case ElementType.Boolean: xmlElement.Value = jsonElement.Value.ToString().ToLower(); break;
                default: xmlElement.Value = jsonElement.Value.ToString(); break;
            }
        }

        private static void SetElementType(XElement xmlElement, ElementType type)
        {
            string typeName = null;
            switch (type)
            {
                case ElementType.Object: typeName = XmlJson.Object; break;
                case ElementType.Array: typeName = XmlJson.Array; break;
                case ElementType.Null: typeName = XmlJson.Null; break;
                case ElementType.String: typeName = XmlJson.String; break;
                case ElementType.Number: typeName = XmlJson.Number; break;
                case ElementType.Boolean: typeName = XmlJson.Boolean; break;
            }
            xmlElement.Add(new XAttribute(XmlJson.TypeAttribute, typeName));
        }
    }
}