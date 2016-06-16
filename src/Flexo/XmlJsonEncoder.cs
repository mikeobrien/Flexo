using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Linq;
using Flexo.Extensions;

namespace Flexo
{
    public class XmlJsonEncoder : IJsonEncoder
    {
        public CultureInfo DefaultCulture = CultureInfo.InvariantCulture;

        public void Encode(JElement jsonElement, Stream stream, Encoding encoding = null, bool pretty = false)
        {
            try
            {
                var xmlElement = new XElement(XmlJson.RootElementName);
                Save(jsonElement, xmlElement);
                using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, encoding ?? Encoding.UTF8, false, pretty))
                {
                    xmlElement.Save(writer);
                    writer.Flush();
                }
            }
            catch (Exception exception)
            {
                throw new JsonEncodeException(exception);
            }
        }

        private void Save(JElement jsonElement, XElement xmlElement)
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

        private void SetElementValue(JElement jsonElement, XElement xmlElement)
        {
            switch (jsonElement.Type)
            {
                case ElementType.Null: return;
                case ElementType.Boolean: xmlElement.Value = jsonElement.Value.ToString().ToLower(); break;
                case ElementType.String: xmlElement.Value = jsonElement.Value.ToString(); break;
                default: xmlElement.Value = SerializeGeneral(jsonElement.Value); break;
            }
        }

        private string SerializeGeneral(object value)
        {
            return !(value is IConvertible) ? null :
                (string)Convert.ChangeType(value, typeof(string), DefaultCulture);
        }

        private static void SetElementType(XElement xmlElement, ElementType type)
        {
            string typeName;
            switch (type)
            {
                case ElementType.Object: typeName = XmlJson.Object; break;
                case ElementType.Array: typeName = XmlJson.Array; break;
                case ElementType.Null: typeName = XmlJson.Null; break;
                case ElementType.String: typeName = XmlJson.String; break;
                case ElementType.Number: typeName = XmlJson.Number; break;
                case ElementType.Boolean: typeName = XmlJson.Boolean; break;
                default: throw new ArgumentException($"Unknown ElementType value '{type}'.");
            }
            xmlElement.Add(new XAttribute(XmlJson.TypeAttribute, typeName));
        }
    }
}