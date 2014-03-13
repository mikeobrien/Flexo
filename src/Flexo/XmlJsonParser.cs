using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Flexo.Extensions;

namespace Flexo
{
    public class XmlJsonParser : IJsonParser
    {
        public JElement Parse(Stream stream, Encoding encoding = null)
        {
            return Load(Exception<XmlException>.Map(() =>
                XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(
                    stream, encoding ?? Encoding.UTF8, new XmlDictionaryReaderQuotas(), x => { })),
                x => new JsonParseException(x)));
        }

        private static JElement Load(XDocument document)
        {
            var jsonElement = new JElement(GetRootType(document.Root));
            Load(jsonElement, document.Root);
            return jsonElement;
        }

        private static void Load(JElement jsonElement, XElement xmlElement)
        {
            if (jsonElement.IsValue) jsonElement.Value = ParseJsonValue(xmlElement, jsonElement.Type);
            else xmlElement.Elements().ForEach(x => Load(jsonElement.Type == ElementType.Array ? jsonElement.AddArrayElement(GetElementType(x)) :
                jsonElement.AddMember(GetName(x), GetElementType(x)), x));
        }

        private static string GetName(XElement element)
        {
            return element.Attribute("item") != null ? element.Attribute("item").Value : element.Name.LocalName;
        }

        private static object ParseJsonValue(XElement xmleElement, ElementType type)
        {
            switch (type)
            {
                case ElementType.Boolean: return bool.Parse(xmleElement.Value);
                case ElementType.Number: return decimal.Parse(xmleElement.Value);
                case ElementType.String: return xmleElement.Value;
                default: return null;
            }
        }

        private static ElementType GetRootType(XElement xmlElement)
        {
            var elementType = GetElementType(xmlElement);
            switch (elementType)
            {
                case ElementType.Object: return ElementType.Object;
                case ElementType.Array: return ElementType.Array;
                default: throw new JsonParseException("'{0}' is not a valid json root element type. " +
                    "The root can only be an object or array.".ToFormat(elementType));
            }
        }

        private static ElementType GetElementType(XElement element)
        {
            var attribute = element.Attribute(XmlJson.TypeAttribute);
            if (attribute == null) throw new InvalidOperationException(
                "Element type missing from '{0}'.".ToFormat(element.GetPath()));
            switch (attribute.Value)
            {
                case XmlJson.Object: return ElementType.Object;
                case XmlJson.Array: return ElementType.Array;
                case XmlJson.Null: return ElementType.Null;
                case XmlJson.String: return ElementType.String;
                case XmlJson.Number: return ElementType.Number;
                case XmlJson.Boolean: return ElementType.Boolean;
                default: throw new JsonParseException(
                    "'{0}' is not a valid json element type.".ToFormat(attribute.Value));
            }
        }
    }
}