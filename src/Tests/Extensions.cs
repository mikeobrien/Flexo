using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using Should;

namespace Tests
{
    public static class Extensions
    {
        public static Stream ToStream(this string value, Encoding encoding = null)
        {
            return new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(value));
        }

        public static Stream ShouldEqual(this Stream stream, string expected)
        {
            new StreamReader(stream).ReadToEnd().ShouldEqual(expected);
            return stream;
        }

        public static XElement ParseJson(this string json)
        {
            return XDocument.Load(JsonReaderWriterFactory.CreateJsonReader(
                new MemoryStream((Encoding.UTF8).GetBytes(json)), 
                Encoding.UTF8, XmlDictionaryReaderQuotas.Max, x => { })).Root;
        }

        public static Stream EncodeJson(this XElement element)
        {
            var stream = new MemoryStream();
            using (var writer = JsonReaderWriterFactory.CreateJsonWriter(stream, Encoding.UTF8, false))
            {
                element.Save(writer);
                writer.Flush();
                stream.Position = 0;
                return stream;
            }
        }
    }
}
