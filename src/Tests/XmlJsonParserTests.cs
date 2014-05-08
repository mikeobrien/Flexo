using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Flexo;
using Flexo.Extensions;
using NUnit.Framework;
using Should;

namespace Tests
{
    [TestFixture]
    public class XmlJsonParserTests
    {
        readonly XmlJsonParser _parser = new XmlJsonParser();

        // Parse Errors

        [Test]
        public void should_fail_on_unclosed_outer_object()
        {
            var exception = Assert.Throws<JsonParseException>(() => _parser.Parse("{".ToStream()));
            #if __MonoCS__
            exception.Message.ShouldEqual("1 missing end of arrays or objects (1,2)");
            #else 
            exception.Message.ShouldEqual("Unexpected end of file. Following elements are not closed: root.");
            #endif
        }

        [Test]
        public void should_fail_on_missing_token()
        {
            var exception = Assert.Throws<JsonParseException>(() => _parser.Parse("{ \"yada\" }".ToStream()));
            #if __MonoCS__
            exception.Message.ShouldEqual("':' is expected after a name of an object content (1,10)");
            #else
            exception.Message.ShouldEqual("The token ':' was expected but found '}'.");
            #endif
        }

        [Test]
        public void should_fail_on_unclosed_nested_array()
        {
            var exception = Assert.Throws<JsonParseException>(() => _parser.Parse("{ \"yada\": [ }".ToStream()));
            #if __MonoCS__
            exception.Message.ShouldEqual("Unexpected end of object (1,13)");
            #else
            exception.Message.ShouldEqual("Encountered unexpected character '}'.");
            #endif
        }

        [Test]
        public void should_fail_on_quoted_json()
        {
            var exception = Assert.Throws<JsonParseException>(() => _parser.Parse("\"{}\"".ToStream()));
            #if __MonoCS__
            exception.Message.ShouldEqual("Unexpected end of object (1,13)");
            #else
            exception.Message.ShouldEqual("A string is not a valid json root element type. The root can only be an object or array.");
            #endif
        }

        // Empty root object

        [Test]
        public void should_read_empty_root_object()
        {
            var element = _parser.Parse("{}".ToStream());
            element.Count().ShouldEqual(0);
            element.IsNamed.ShouldBeFalse();
            element.Type.ShouldEqual(ElementType.Object);
        }

        // Empty root array

        [Test]
        public void should_read_empty_root_array()
        {
            var element = _parser.Parse("[]".ToStream());
            element.Count().ShouldEqual(0);
            element.IsNamed.ShouldBeFalse();
            element.Type.ShouldEqual(ElementType.Array);
        }

        // Field names
        
        [Test]
        public void should_parse_field_names_with_non_alpha_numeric_chars()
        {
            #if __MonoCS__
            // Broken in mono as of 2.10.8.1
            // https://bugzilla.xamarin.com/show_bug.cgi?id=18105
            #else
            var children = _parser.Parse("{ \"$field1\": \"hai\" }".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].Name.ShouldEqual("$field1");
            children[0].Value.ShouldEqual("hai");
            children[0].Type.ShouldEqual(ElementType.String);
            #endif
        }

        // String values

        [Test]
        public void should_get_field_string_value()
        {
            var children = _parser.Parse("{ \"field1\": \"hai\" }".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].Name.ShouldEqual("field1");
            children[0].Value.ShouldEqual("hai");
            children[0].Type.ShouldEqual(ElementType.String);
        }

        [Test]
        public void should_get_field_escaped_string_value()
        {
            var children = _parser.Parse("{{ \"field1\": \"{0}\" }}"
                .ToFormat(DateTime.MaxValue).ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].Name.ShouldEqual("field1");
            children[0].Value.ShouldEqual(DateTime.MaxValue.ToString());
            children[0].Type.ShouldEqual(ElementType.String);
        }

        [Test]
        public void should_get_array_string_value()
        {
            var children = _parser.Parse("[\"hai\"]".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].IsNamed.ShouldBeFalse();
            children[0].Value.ShouldEqual("hai");
            children[0].Type.ShouldEqual(ElementType.String);
        }

        [Test]
        public void should_get_array_escaped_string_value()
        {
            var children = _parser.Parse("[\"{0}\"]"
                .ToFormat(DateTime.MaxValue).ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].IsNamed.ShouldBeFalse();
            children[0].Value.ShouldEqual(DateTime.MaxValue.ToString());
            children[0].Type.ShouldEqual(ElementType.String);
        }

        // Numeric values

        [Test]
        public void should_get_field_number_value()
        {
            var children = _parser.Parse("{ \"field1\": 42 }".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].Name.ShouldEqual("field1");
            children[0].Value.ShouldEqual(42.0m);
            children[0].Type.ShouldEqual(ElementType.Number);
        }

        [Test]
        public void should_get_array_number_value()
        {
            var children = _parser.Parse("[42]".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].IsNamed.ShouldBeFalse();
            children[0].Value.ShouldEqual(42.0m);
            children[0].Type.ShouldEqual(ElementType.Number);
        }

        // Bool values

        [Test]
        public void should_get_field_bool_value()
        {
            var children = _parser.Parse("{ \"field1\": true }".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].Name.ShouldEqual("field1");
            children[0].Value.ShouldEqual(true);
            children[0].Type.ShouldEqual(ElementType.Boolean);
        }

        [Test]
        public void should_get_array_bool_value()
        {
            var children = _parser.Parse("[true]".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].IsNamed.ShouldBeFalse();
            children[0].Value.ShouldEqual(true);
            children[0].Type.ShouldEqual(ElementType.Boolean);
        }

        // Null values

        [Test]
        public void should_get_field_null_value()
        {
            var children = _parser.Parse("{ \"field1\": null }".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].Name.ShouldEqual("field1");
            children[0].Value.ShouldEqual(null);
            children[0].Type.ShouldEqual(ElementType.Null);
        }

        [Test]
        public void should_get_array_null_value()
        {
            var children = _parser.Parse("[null]".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].IsNamed.ShouldBeFalse();
            children[0].Value.ShouldEqual(null);
            children[0].Type.ShouldEqual(ElementType.Null);
        }

        // Array values

        [Test]
        public void should_get_field_empty_array_value()
        {
            var children = _parser.Parse("{ \"field1\": [] }".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].Name.ShouldEqual("field1");
            children[0].Type.ShouldEqual(ElementType.Array);
        }

        [Test]
        public void should_get_array_empty_array_value()
        {
            var children = _parser.Parse("[[]]".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].IsNamed.ShouldBeFalse();
            children[0].Type.ShouldEqual(ElementType.Array);
        }

        [Test]
        public void should_get_field_array_value()
        {
            var children = _parser.Parse("{ \"field1\": [42, \"hai\"] }".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            var child = children[0];
            child.Name.ShouldEqual("field1");
            child.Type.ShouldEqual(ElementType.Array);

            children = child.ToList();
            children.Count.ShouldEqual(2);
            child = children[0];
            child.IsNamed.ShouldBeFalse();
            child.Value.ShouldEqual(42m);
            child.Type.ShouldEqual(ElementType.Number);
            child = children[1];
            child.IsNamed.ShouldBeFalse();
            child.Value.ShouldEqual("hai");
            child.Type.ShouldEqual(ElementType.String);
        }

        [Test]
        public void should_get_array_array_value()
        {
            var children = _parser.Parse("[[42, \"hai\"]]".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            var child = children[0];
            child.IsNamed.ShouldBeFalse();
            child.Type.ShouldEqual(ElementType.Array);

            children = child.ToList();
            children.Count.ShouldEqual(2);
            child = children[0];
            child.IsNamed.ShouldBeFalse();
            child.Value.ShouldEqual(42m);
            child.Type.ShouldEqual(ElementType.Number);
            child = children[1];
            child.IsNamed.ShouldBeFalse();
            child.Value.ShouldEqual("hai");
            child.Type.ShouldEqual(ElementType.String);
        }

        // Object values

        [Test]
        public void should_get_field_empty_object_value()
        {
            var children = _parser.Parse("{ \"field1\": {} }".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].Name.ShouldEqual("field1");
            children[0].Type.ShouldEqual(ElementType.Object);
        }

        [Test]
        public void should_get_array_empty_object_value()
        {
            var children = _parser.Parse("[{}]".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            children[0].IsNamed.ShouldBeFalse();
            children[0].Type.ShouldEqual(ElementType.Object);
        }

        [Test]
        public void should_get_field_object_value()
        {
            var children = _parser.Parse("{ \"field1\": { \"field2\": 42, \"field3\": \"hai\" } }".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            var child = children[0];
            child.Name.ShouldEqual("field1");
            child.Type.ShouldEqual(ElementType.Object);

            children = child.ToList();
            children.Count.ShouldEqual(2);
            child = children[0];
            child.Name.ShouldEqual("field2");
            child.Value.ShouldEqual(42m);
            child.Type.ShouldEqual(ElementType.Number);
            child = children[1];
            child.Name.ShouldEqual("field3");
            child.Value.ShouldEqual("hai");
            child.Type.ShouldEqual(ElementType.String);
        }

        [Test]
        public void should_get_array_object_value()
        {
            var children = _parser.Parse("[{ \"field1\": 42, \"field2\": \"hai\" }]".ToStream()).ToList();
            children.Count.ShouldEqual(1);
            var child = children[0];
            child.IsNamed.ShouldBeFalse();
            child.Type.ShouldEqual(ElementType.Object);

            children = child.ToList();
            children.Count.ShouldEqual(2);
            child = children[0];
            child.Name.ShouldEqual("field1");
            child.Value.ShouldEqual(42m);
            child.Type.ShouldEqual(ElementType.Number);
            child = children[1];
            child.Name.ShouldEqual("field2");
            child.Value.ShouldEqual("hai");
            child.Type.ShouldEqual(ElementType.String);
        }

        // Multiple values

        [Test]
        public void should_get_multiple_fields()
        {
            var children = _parser.Parse("{ \"field1\": \"oh\", \"field2\": \"hai\" }".ToStream()).ToList();
            children.Count.ShouldEqual(2);
            children[0].Name.ShouldEqual("field1");
            children[0].Value.ShouldEqual("oh");
            children[0].Type.ShouldEqual(ElementType.String);
            children[1].Name.ShouldEqual("field2");
            children[1].Value.ShouldEqual("hai");
            children[1].Type.ShouldEqual(ElementType.String);
        }

        [Test]
        public void should_get_multiple_array_elements()
        {
            var children = _parser.Parse("[\"oh\", \"hai\"]".ToStream()).ToList();
            children.Count.ShouldEqual(2);
            children[0].IsNamed.ShouldBeFalse();
            children[0].Value.ShouldEqual("oh");
            children[0].Type.ShouldEqual(ElementType.String);
            children[0].IsNamed.ShouldBeFalse();
            children[1].Value.ShouldEqual("hai");
            children[1].Type.ShouldEqual(ElementType.String);
        }

        // Whitespace

        [Test]
        public void should_read_fields_with_whitespace()
        {
            var children = _parser.Parse("{\r\n    \"field1\": \"oh\",\r\n\t\"field2\": \"hai\"\r\n}".ToStream()).ToList();
            children.Count.ShouldEqual(2);
            children[0].Name.ShouldEqual("field1");
            children[0].Value.ShouldEqual("oh");
            children[0].Type.ShouldEqual(ElementType.String);
            children[1].Name.ShouldEqual("field2");
            children[1].Value.ShouldEqual("hai");
            children[1].Type.ShouldEqual(ElementType.String);
        }

        [Test]
        public void should_read_array_elements_with_whitespace()
        {
            var children = _parser.Parse("[\r\n    \"oh\",\r\n\t\"hai\"\r\n]".ToStream()).ToList();
            children.Count.ShouldEqual(2);
            children[0].IsNamed.ShouldBeFalse();
            children[0].Value.ShouldEqual("oh");
            children[0].Type.ShouldEqual(ElementType.String);
            children[0].IsNamed.ShouldBeFalse();
            children[1].Value.ShouldEqual("hai");
            children[1].Type.ShouldEqual(ElementType.String);
        }

        // Performance

        [Test]
        public void should_be_within_performace_tolerance()
        {
            var json = File.ReadAllText("model.json");
            var jsonBytes = new MemoryStream(File.ReadAllBytes("model.json"));
            var stopwatch = new Stopwatch();

            var controlBenchmark = Enumerable.Range(1, 1000).Select(x =>
            {
                jsonBytes.Seek(0, SeekOrigin.Begin);
                stopwatch.Restart();
                json.ParseJson();
                stopwatch.Stop();
                return stopwatch.ElapsedTicks;
            }).Skip(5).Average();

            var flexoBenchmark = Enumerable.Range(1, 1000).Select(x =>
            {
                jsonBytes.Seek(0, SeekOrigin.Begin);
                stopwatch.Restart();
                JElement.Load(jsonBytes);
                stopwatch.Stop();
                return stopwatch.ElapsedTicks;
            }).Skip(5).Average();

            Console.Write("Control: {0}, Flexo: {1}", controlBenchmark, flexoBenchmark);

            flexoBenchmark.ShouldBeLessThan(controlBenchmark * 2);
        }
    }
}
