using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Flexo;
using NUnit.Framework;
using Should;

namespace Tests
{
    [TestFixture]
    public class XmlJsonEncoderTests
    {
        XmlJsonEncoder _encoder = new XmlJsonEncoder();

        // Empty root object

        [Test]
        public void should_create_empty_root_object()
        {
            _encoder.Encode(new JElement(ElementType.Object)).ShouldEqual("{}");
        }

        // Empty root array

        [Test]
        public void should_create_empty_root_array()
        {
            _encoder.Encode(new JElement(ElementType.Array)).ShouldEqual("[]");
        }

        // String values

        [Test]
        public void should_set_field_string_value()
        {
            var element = new JElement(ElementType.Object);
            element.AddValueMember("field1", "hai");
            element.AddValueMember("field2", 'y');
            _encoder.Encode(element).ShouldEqual("{\"field1\":\"hai\",\"field2\":\"y\"}");
        }

        [Test]
        public void should_set_array_string_elements()
        {
            var element = new JElement(ElementType.Array);
            element.Type = ElementType.Array;
            element.AddArrayValueElement("hai");
            element.AddArrayValueElement('y');
            _encoder.Encode(element).ShouldEqual("[\"hai\",\"y\"]");
        }

        // Numeric values

        [Test]
        public void should_set_field_number_values()
        {
            var element = new JElement(ElementType.Object);
            element.AddValueMember("field1", (decimal)1.1);
            element.AddValueMember("field2", (float)2.2);
            element.AddValueMember("field3", (double)3.3);
            element.AddValueMember("field4", (sbyte)4);
            element.AddValueMember("field5", (byte)5);
            element.AddValueMember("field6", (short)6);
            element.AddValueMember("field7", (ushort)7);
            element.AddValueMember("field8", (int)8);
            element.AddValueMember("field9", (uint)9);
            element.AddValueMember("field10", (long)10);
            element.AddValueMember("field11", (ulong)11);
            _encoder.Encode(element).ShouldEqual("{" +
                "\"field1\":1.1," +
                "\"field2\":2.2," +
                "\"field3\":3.3," +
                "\"field4\":4," +
                "\"field5\":5," +
                "\"field6\":6," +
                "\"field7\":7," +
                "\"field8\":8," +
                "\"field9\":9," +
                "\"field10\":10," +
                "\"field11\":11" +
                "}");
        }

        [Test]
        public void should_set_array_number_elements()
        {
            var element = new JElement(ElementType.Array);
            element.Type = ElementType.Array;
            element.AddArrayValueElement((decimal)1.1);
            element.AddArrayValueElement((float)2.2);
            element.AddArrayValueElement((double)3.3);
            element.AddArrayValueElement((sbyte)4);
            element.AddArrayValueElement((byte)5);
            element.AddArrayValueElement((short)6);
            element.AddArrayValueElement((ushort)7);
            element.AddArrayValueElement((int)8);
            element.AddArrayValueElement((uint)9);
            element.AddArrayValueElement((long)10);
            element.AddArrayValueElement((ulong)11);
            _encoder.Encode(element).ShouldEqual("[1.1,2.2,3.3,4,5,6,7,8,9,10,11]");
        }

        // Bool values

        [Test]
        public void should_set_field_bool_value()
        {
            var element = new JElement(ElementType.Object);
            element.AddValueMember("field1", true);
            _encoder.Encode(element).ShouldEqual("{\"field1\":true}");
        }

        [Test]
        public void should_set_array_bool_element()
        {
            var element = new JElement(ElementType.Array);
            element.AddArrayValueElement(true);
            _encoder.Encode(element).ShouldEqual("[true]");
        }

        // Null values

        [Test]
        public void should_set_field_null_value()
        {
            var element = new JElement(ElementType.Object);
            element.AddValueMember("field1", null);
            _encoder.Encode(element).ShouldEqual("{\"field1\":null}");
        }

        [Test]
        public void should_set_array_null_element()
        {
            var element = new JElement(ElementType.Array);
            element.AddArrayValueElement(null);
            _encoder.Encode(element).ShouldEqual("[null]");
        }

        // Array values

        [Test]
        public void should_set_field_empty_array_value()
        {
            var element = new JElement(ElementType.Object);
            element.AddMember("field1", ElementType.Array);
            _encoder.Encode(element).ShouldEqual("{\"field1\":[]}");
        }

        [Test]
        public void should_set_array_empty_array_element()
        {
            var element = new JElement(ElementType.Array);
            element.AddArrayElement(ElementType.Array);
            _encoder.Encode(element).ShouldEqual("[[]]");
        }

        [Test]
        public void should_set_field_array_value()
        {
            var element = new JElement(ElementType.Object);
            var array = element.AddMember("field1", ElementType.Array);
            array.AddArrayValueElement(1);
            array.AddArrayValueElement("hai");
            _encoder.Encode(element).ShouldEqual("{\"field1\":[1,\"hai\"]}");
        }

        [Test]
        public void should_set_array_array_element()
        {
            var element = new JElement(ElementType.Array);
            var array = element.AddArrayElement(ElementType.Array);
            array.AddArrayValueElement(1);
            array.AddArrayValueElement("hai");
            _encoder.Encode(element).ShouldEqual("[[1,\"hai\"]]");
        }

        // Object values

        [Test]
        public void should_set_field_empty_object_value()
        {
            var element = new JElement(ElementType.Object);
            element.AddMember("field1", ElementType.Object);
            _encoder.Encode(element).ShouldEqual("{\"field1\":{}}");
        }

        [Test]
        public void should_set_array_empty_object_element()
        {
            var element = new JElement(ElementType.Array);
            element.AddArrayElement(ElementType.Object);
            _encoder.Encode(element).ShouldEqual("[{}]");
        }

        [Test]
        public void should_set_field_object_value()
        {
            var element = new JElement(ElementType.Object);
            var @object = element.AddMember("field1", ElementType.Object);
            @object.AddValueMember("field2", 1);
            @object.AddValueMember("field3", "hai");
            _encoder.Encode(element).ShouldEqual("{\"field1\":{\"field2\":1,\"field3\":\"hai\"}}");
        }

        [Test]
        public void should_set_array_object_element()
        {
            var element = new JElement(ElementType.Array);
            var @object = element.AddArrayElement(ElementType.Object);
            @object.AddValueMember("field2", 1);
            @object.AddValueMember("field3", "hai");
            _encoder.Encode(element).ShouldEqual("[{\"field2\":1,\"field3\":\"hai\"}]");
        }

        // Multiple values

        [Test]
        public void should_set_multiple_fields()
        {
            var element = new JElement(ElementType.Object);
            element.AddValueMember("field1", "oh");
            element.AddValueMember("field2", "hai");
            _encoder.Encode(element).ShouldEqual("{\"field1\":\"oh\",\"field2\":\"hai\"}");
        }

        [Test]
        public void should_set_multiple_array_elements()
        {
            var element = new JElement(ElementType.Array);
            element.AddArrayValueElement("oh");
            element.AddArrayValueElement("hai");
            _encoder.Encode(element).ShouldEqual("[\"oh\",\"hai\"]");
        }

        // Whitespace

        [Test, Ignore("Whitespace is not supported in 4.0 but is supported in 4.5.")]
        public void should_save_with_whitespace()
        {
            var element = new JElement(ElementType.Object);
            element.AddValueMember("field1", "hai");
            _encoder.Encode(element).ShouldEqual("{\r\n  \"field1\": \"hai\"\r\n}");
        }

        // Performance

        [Test]
        public void should_be_within_performace_tolerance()
        {
            var xml = File.ReadAllText("model.json").ParseJson();
            var json = JElement.Load(File.ReadAllBytes("model.json"));
            var stopwatch = new Stopwatch();

            var controlBenchmark = Enumerable.Range(1, 1000).Select(x =>
            {
                stopwatch.Restart();
                xml.EncodeJson();
                stopwatch.Stop();
                return stopwatch.ElapsedTicks;
            }).Skip(5).Average();

            var flexoBenchmark = Enumerable.Range(1, 1000).Select(x =>
            {
                stopwatch.Restart();
                json.Encode();
                stopwatch.Stop();
                return stopwatch.ElapsedTicks;
            }).Skip(5).Average();

            Console.Write("Control: {0}, Flexo: {1}", controlBenchmark, flexoBenchmark);

            flexoBenchmark.ShouldBeLessThan(controlBenchmark * 3);
        }
    }
}
