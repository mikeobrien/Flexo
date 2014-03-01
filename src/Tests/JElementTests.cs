using System.IO;
using System.Linq;
using System.Text;
using Flexo;
using NUnit.Framework;
using Should;

namespace Tests
{
    [TestFixture]
    public class JElementTests
    {
        // Parse/Encode

        [Test]
        public void should_parse_json_string()
        {
            var element = JElement.Load("{}");

            element.ShouldBeRoot();
            element.ShouldBeAJsonObject();
            element.ShouldBeEmpty();
        }

        private readonly static byte[] JsonBytesUtf8 = Encoding.UTF8.GetBytes("{}");
        private readonly static byte[] JsonBytesUnicode = Encoding.BigEndianUnicode.GetBytes("{}");

        [Test]
        public void should_parse_with_json_bytes()
        {
            var element = JElement.Load(JsonBytesUtf8);

            element.ShouldBeRoot();
            element.ShouldBeAJsonObject();
            element.ShouldBeEmpty();
        }

        [Test]
        public void should_parse_with_json_bytes_with_specified_encoding()
        {
            var element = JElement.Load(JsonBytesUnicode, Encoding.BigEndianUnicode);

            element.ShouldBeRoot();
            element.ShouldBeAJsonObject();
            element.ShouldBeEmpty();
        }

        [Test]
        public void should_parse_with_json_stream()
        {
            var element = JElement.Load(new MemoryStream(JsonBytesUtf8));

            element.ShouldBeRoot();
            element.ShouldBeAJsonObject();
            element.ShouldBeEmpty();
        }

        [Test]
        public void should_parse_with_json_stream_with_specified_encoding()
        {
            var element = JElement.Load(new MemoryStream(JsonBytesUnicode), Encoding.BigEndianUnicode);

            element.ShouldBeRoot();
            element.ShouldBeAJsonObject();
            element.ShouldBeEmpty();
        }

        [Test]
        public void should_encode_json_string()
        {
            JElement.Create(ElementType.Object).ToString().ShouldEqual("{}");
        }

        [Test]
        public void should_encode_json_stream()
        {
            JElement.Create(ElementType.Object).Encode().ShouldEqual("{}");
        }

        // Path

        [Test]
        public void should_return_path_for_object()
        {
            var element = JElement.Create(ElementType.Object);
            
            element.Path.ShouldEqual("");

            element.AddMember("boolField", ElementType.Boolean).Path.ShouldEqual("boolField");
            element.AddMember("nullField", ElementType.Null).Path.ShouldEqual("nullField");
            element.AddMember("numberField", ElementType.Number).Path.ShouldEqual("numberField");
            element.AddMember("stringField", ElementType.String).Path.ShouldEqual("stringField");

            var arrayField = element.AddMember("arrayField", ElementType.Array);
            arrayField.Path.ShouldEqual("arrayField");

            arrayField.AddArrayElement(ElementType.Boolean).Path.ShouldEqual("arrayField[1]");
            arrayField.AddArrayElement(ElementType.Null).Path.ShouldEqual("arrayField[2]");
            arrayField.AddArrayElement(ElementType.Number).Path.ShouldEqual("arrayField[3]");
            arrayField.AddArrayElement(ElementType.String).Path.ShouldEqual("arrayField[4]");
            arrayField.AddArrayElement(ElementType.Array).Path.ShouldEqual("arrayField[5]");
            arrayField.AddArrayElement(ElementType.Object).Path.ShouldEqual("arrayField[6]");

            var objectField = element.AddMember("objectField", ElementType.Object);
            objectField.Path.ShouldEqual("objectField");

            objectField.AddMember("boolField2", ElementType.Boolean).Path.ShouldEqual("objectField.boolField2");
            objectField.AddMember("nullField2", ElementType.Null).Path.ShouldEqual("objectField.nullField2");
            objectField.AddMember("numberField2", ElementType.Number).Path.ShouldEqual("objectField.numberField2");
            objectField.AddMember("stringField2", ElementType.String).Path.ShouldEqual("objectField.stringField2");
            objectField.AddMember("arrayField2", ElementType.Array).Path.ShouldEqual("objectField.arrayField2");
            objectField.AddMember("objectField2", ElementType.Object).Path.ShouldEqual("objectField.objectField2");
        }

        [Test]
        public void should_return_path_for_array()
        {
            var element = JElement.Create(ElementType.Array);

            element.Path.ShouldEqual("[]");

            element.AddArrayElement(ElementType.Boolean).Path.ShouldEqual("[1]");
            element.AddArrayElement(ElementType.Null).Path.ShouldEqual("[2]");
            element.AddArrayElement(ElementType.Number).Path.ShouldEqual("[3]");
            element.AddArrayElement(ElementType.String).Path.ShouldEqual("[4]");

            var arrayField = element.AddArrayElement(ElementType.Array);
            arrayField.Path.ShouldEqual("[5]");

            arrayField.AddArrayElement(ElementType.Boolean).Path.ShouldEqual("[5][1]");
            arrayField.AddArrayElement(ElementType.Null).Path.ShouldEqual("[5][2]");
            arrayField.AddArrayElement(ElementType.Number).Path.ShouldEqual("[5][3]");
            arrayField.AddArrayElement(ElementType.String).Path.ShouldEqual("[5][4]");
            arrayField.AddArrayElement(ElementType.Array).Path.ShouldEqual("[5][5]");
            arrayField.AddArrayElement(ElementType.Object).Path.ShouldEqual("[5][6]");

            var objectField = element.AddArrayElement(ElementType.Object);
            objectField.Path.ShouldEqual("[6]");

            objectField.AddMember("boolField2", ElementType.Boolean).Path.ShouldEqual("[6].boolField2");
            objectField.AddMember("nullField2", ElementType.Null).Path.ShouldEqual("[6].nullField2");
            objectField.AddMember("numberField2", ElementType.Number).Path.ShouldEqual("[6].numberField2");
            objectField.AddMember("stringField2", ElementType.String).Path.ShouldEqual("[6].stringField2");
            objectField.AddMember("arrayField2", ElementType.Array).Path.ShouldEqual("[6].arrayField2");
            objectField.AddMember("objectField2", ElementType.Object).Path.ShouldEqual("[6].objectField2");
        }

        // Create

        [Test]
        public void should_create_object_root()
        {
            var element = JElement.Create(ElementType.Object);

            element.ShouldBeRoot();
            element.ShouldBeAJsonObject();
            element.ShouldBeEmpty();
        }

        [Test]
        public void should_create_array_root()
        {
            var element = JElement.Create(ElementType.Array);

            element.ShouldBeRoot();
            element.ShouldBeAJsonArray();
            element.ShouldBeEmpty();
        }

        // Add fields

        [Test]
        public void should_add_fields_with_default_values()
        {
            var element = new JElement(ElementType.Object);
            element.AddMember("field1", ElementType.Boolean);
            element.AddMember("field2", ElementType.Number);
            element.AddMember("field3", ElementType.String);
            element.AddMember("field4", ElementType.Null);
            element.AddMember("field5", ElementType.Object);
            element.AddMember("field6", ElementType.Array);

            element.Count().ShouldEqual(6);
            var fields = element.ToList();
            fields[0].ShouldBeAJsonBoolValueField("field1", false);
            fields[1].ShouldBeAJsonNumberValueField("field2", 0);
            fields[2].ShouldBeAJsonStringValueField("field3", "");
            fields[3].ShouldBeAJsonNullValueField("field4");
            fields[4].ShouldBeAJsonObjectField("field5");
            fields[5].ShouldBeAJsonArrayField("field6");
        }

        [Test]
        public void should_add_value_fields_with_explicit_values()
        {
            var element = new JElement(ElementType.Object);
            element.AddValueMember("field1", true);
            element.AddValueMember("field2", 5);
            element.AddValueMember("field3", "hai");
            element.AddValueMember("field4", null);

            element.Count().ShouldEqual(4);
            var fields = element.ToList();
            fields[0].ShouldBeAJsonBoolValueField("field1", true);
            fields[1].ShouldBeAJsonNumberValueField("field2", 5);
            fields[2].ShouldBeAJsonStringValueField("field3", "hai");
            fields[3].ShouldBeAJsonNullValueField("field4");
        }

        [Test]
        public void should_fail_to_add_fields_to_an_array()
        {
            var array = JElement.Create(ElementType.Array);
            array.IsArray.ShouldBeTrue();
            Assert.Throws<JsonMemberNotSupportedException>(() => array.AddMember("field", ElementType.Null));
            Assert.Throws<JsonMemberNotSupportedException>(() => array.AddMember("field", 0));
        }

        // Insert fields

        [Test]
        public void should_insert_fields_with_default_values()
        {
            var element = new JElement(ElementType.Object);

            var boolElement = new JElement("field1", ElementType.Boolean);
            element.CanInsert(boolElement).ShouldBeTrue();
            element.Insert(boolElement);

            var numberElement = new JElement("field2", ElementType.Number);
            element.CanInsert(numberElement).ShouldBeTrue();
            element.Insert(numberElement);

            var stringElement = new JElement("field3", ElementType.String);
            element.CanInsert(boolElement).ShouldBeTrue();
            element.Insert(stringElement);

            var nullElement = new JElement("field4", ElementType.Null);
            element.CanInsert(boolElement).ShouldBeTrue();
            element.Insert(nullElement);

            var objectElement = new JElement("field5", ElementType.Object);
            element.CanInsert(boolElement).ShouldBeTrue();
            element.Insert(objectElement);

            var arrayElement = new JElement("field6", ElementType.Array);
            element.CanInsert(boolElement).ShouldBeTrue();
            element.Insert(arrayElement);

            element.Count().ShouldEqual(6);
            var fields = element.ToList();

            fields[0].ShouldBeAJsonBoolValueField("field1");
            fields[0].Parent.ShouldBeSameAs(element);

            fields[1].ShouldBeAJsonNumberValueField("field2");
            fields[1].Parent.ShouldBeSameAs(element);

            fields[2].ShouldBeAJsonStringValueField("field3");
            fields[2].Parent.ShouldBeSameAs(element);

            fields[3].ShouldBeAJsonNullValueField("field4");
            fields[3].Parent.ShouldBeSameAs(element);

            fields[4].ShouldBeAJsonObjectField("field5");
            fields[4].Parent.ShouldBeSameAs(element);

            fields[5].ShouldBeAJsonArrayField("field6");
            fields[5].Parent.ShouldBeSameAs(element);

        }

        [Test]
        public void should_insert_value_fields_with_explicit_values()
        {
            var element = new JElement(ElementType.Object);

            var boolElement = new JElement("field1", true, ValueElementType.Boolean);
            element.CanInsert(boolElement).ShouldBeTrue();
            element.Insert(boolElement);

            var numberElement = new JElement("field2", 5, ValueElementType.Number);
            element.CanInsert(numberElement).ShouldBeTrue();
            element.Insert(numberElement);

            var stringElement = new JElement("field3", "hai", ValueElementType.String);
            element.CanInsert(boolElement).ShouldBeTrue();
            element.Insert(stringElement);

            element.Count().ShouldEqual(3);
            var fields = element.ToList();

            fields[0].ShouldBeAJsonBoolValueField("field1", true);
            fields[0].Parent.ShouldBeSameAs(element);

            fields[1].ShouldBeAJsonNumberValueField("field2", 5);
            fields[1].Parent.ShouldBeSameAs(element);

            fields[2].ShouldBeAJsonStringValueField("field3", "hai");
            fields[2].Parent.ShouldBeSameAs(element);
        }

        // Add array elements

        [Test]
        public void should_add_array_elements_with_default_values()
        {
            var element = new JElement(ElementType.Array);
            element.AddArrayElement(ElementType.Boolean);
            element.AddArrayElement(ElementType.Number);
            element.AddArrayElement(ElementType.String);
            element.AddArrayElement(ElementType.Null);
            element.AddArrayElement(ElementType.Object);
            element.AddArrayElement(ElementType.Array);

            element.Count().ShouldEqual(6);
            var elements = element.ToList();
            elements[0].ShouldBeAJsonBoolValueArrayElement(false);
            elements[1].ShouldBeAJsonNumberValueArrayElement(0);
            elements[2].ShouldBeAJsonStringValueArrayElement("");
            elements[3].ShouldBeAJsonNullValueArrayElement();
            elements[4].ShouldBeAJsonObjectArrayElement();
            elements[5].ShouldBeAJsonArrayArrayElement();
        }

        [Test]
        public void should_add_value_array_elements_with_explicit_values()
        {
            var element = new JElement(ElementType.Array);
            element.AddArrayValueElement(true);
            element.AddArrayValueElement(5);
            element.AddArrayValueElement("hai");
            element.AddArrayValueElement(null);

            element.Count().ShouldEqual(4);
            var elements = element.ToList();
            elements[0].ShouldBeAJsonBoolValueArrayElement(true);
            elements[1].ShouldBeAJsonNumberValueArrayElement(5);
            elements[2].ShouldBeAJsonStringValueArrayElement("hai");
            elements[3].ShouldBeAJsonNullValueArrayElement();
        }

        [Test]
        public void should_fail_to_add_array_elements_to_an_object()
        {
            var @object = JElement.Create(ElementType.Object);
            @object.IsObject.ShouldBeTrue();
            Assert.Throws<JsonArrayElementNotSupportedException>(() => @object.AddArrayElement(ElementType.Null));
            Assert.Throws<JsonArrayElementNotSupportedException>(() => @object.AddArrayElement(0));
        }

        // Insert array elements

        [Test]
        public void should_insert_array_elements_with_default_values()
        {
            var element = new JElement(ElementType.Array);

            var boolElement = new JElement(ElementType.Boolean);
            element.CanInsert(boolElement).ShouldBeTrue();
            element.Insert(boolElement);

            var numberElement = new JElement(ElementType.Number);
            element.CanInsert(numberElement).ShouldBeTrue();
            element.Insert(numberElement);

            var stringElement = new JElement(ElementType.String);
            element.CanInsert(stringElement).ShouldBeTrue();
            element.Insert(stringElement);

            var nullElement = new JElement(ElementType.Null);
            element.CanInsert(nullElement).ShouldBeTrue();
            element.Insert(nullElement);

            var objectElement = new JElement(ElementType.Object);
            element.CanInsert(objectElement).ShouldBeTrue();
            element.Insert(objectElement);

            var arrayElement = new JElement(ElementType.Array);
            element.CanInsert(arrayElement).ShouldBeTrue();
            element.Insert(arrayElement);

            element.Count().ShouldEqual(6);
            var elements = element.ToList();

            elements[0].ShouldBeAJsonBoolValueArrayElement();
            elements[0].Parent.ShouldBeSameAs(element);

            elements[1].ShouldBeAJsonNumberValueArrayElement();
            elements[1].Parent.ShouldBeSameAs(element);

            elements[2].ShouldBeAJsonStringValueArrayElement();
            elements[2].Parent.ShouldBeSameAs(element);

            elements[3].ShouldBeAJsonNullValueArrayElement();
            elements[3].Parent.ShouldBeSameAs(element);

            elements[4].ShouldBeAJsonObjectArrayElement();
            elements[4].Parent.ShouldBeSameAs(element);

            elements[5].ShouldBeAJsonArrayArrayElement();
            elements[5].Parent.ShouldBeSameAs(element);
        }

        [Test]
        public void should_insert_value_array_elements_with_explicit_values()
        {
            var element = new JElement(ElementType.Array);

            var boolElement = new JElement(true, ValueElementType.Boolean);
            element.CanInsert(boolElement).ShouldBeTrue();
            element.Insert(boolElement);

            var numberElement = new JElement(5, ValueElementType.Number);
            element.CanInsert(numberElement).ShouldBeTrue();
            element.Insert(numberElement);

            var stringElement = new JElement("hai", ValueElementType.String);
            element.CanInsert(stringElement).ShouldBeTrue();
            element.Insert(stringElement);

            element.Count().ShouldEqual(3);
            var elements = element.ToList();

            elements[0].ShouldBeAJsonBoolValueArrayElement(true);
            elements[0].Parent.ShouldBeSameAs(element);

            elements[1].ShouldBeAJsonNumberValueArrayElement(5);
            elements[1].Parent.ShouldBeSameAs(element);

            elements[2].ShouldBeAJsonStringValueArrayElement("hai");
            elements[2].Parent.ShouldBeSameAs(element);
        }

        [Test]
        public void should_insert_value_named_array_element_and_clear_name()
        {
            new JElement(ElementType.Array).Insert(
                new JElement("hai", ElementType.Boolean)).IsNamed.ShouldBeFalse();
        }

        [Test]
        public void should_insert_value_array_element_and_not_clear_name()
        {
            new JElement(ElementType.Array).Insert(
                new JElement(ElementType.Boolean)).IsNamed.ShouldBeFalse();
        }

        [Test]
        public void should_fail_to_insert_array_items_to_an_object()
        {
            Assert.Throws<JsonUnnamedElementsNotSupportedException>(() =>
                JElement.Create(ElementType.Object).Insert(new JElement(ElementType.Null)));
        }

        // Names

        [Test]
        public void should_get_and_set_field_name()
        {
            var field = JElement.Create(ElementType.Object).AddMember("field", ElementType.Null);
            field.IsNamed.ShouldBeTrue();
            field.Name.ShouldEqual("field");
            field.Name = "field2";
            field.Name.ShouldEqual("field2");
        }

        [Test]
        public void should_fail_to_get_and_set_array_element_name()
        {
            var element = JElement.Create(ElementType.Array).AddArrayElement(ElementType.Null);
            element.IsNamed.ShouldBeFalse();
            Assert.Throws<JsonNameNotSupportedException>(() => { var name = element.Name; });
            Assert.Throws<JsonNameNotSupportedException>(() => element.Name = "field2");
        }

        [Test]
        [TestCase(ElementType.Object)]
        [TestCase(ElementType.Array)]
        [TestCase(ElementType.Null)]
        [TestCase(ElementType.Number)]
        [TestCase(ElementType.String)]
        [TestCase(ElementType.Boolean)]
        public void should_get_and_set_root_name_when_set_by_constructor(ElementType type)
        {
            var @object = new JElement("oh", type);
            @object.IsRoot.ShouldBeTrue();
            @object.IsNamed.ShouldBeTrue();
            @object.Name.ShouldEqual("oh");
            @object.Name = "hai";
            @object.Name.ShouldEqual("hai");
        }

        [Test]
        [TestCase(ElementType.Object)]
        [TestCase(ElementType.Array)]
        [TestCase(ElementType.Null)]
        [TestCase(ElementType.Number)]
        [TestCase(ElementType.String)]
        [TestCase(ElementType.Boolean)]
        public void should_fail_to_get_and_set_root_array_name(ElementType type)
        {
            var element = JElement.Create(type);
            element.IsRoot.ShouldBeTrue();
            element.IsNamed.ShouldBeFalse();
            Assert.Throws<JsonNameNotSupportedException>(() => { var name = element.Name; });
            Assert.Throws<JsonNameNotSupportedException>(() => element.Name = "field2");
        }

        // Values

        [Test]
        public void should_get_and_set_field_value()
        {
            var field = JElement.Create(ElementType.Object).AddMember("field", ElementType.Null);
            field.IsNull.ShouldBeTrue();
            field.Value.ShouldBeNull();
            field.Value = "hai";
            field.IsString.ShouldBeTrue();
            field.Value.ShouldEqual("hai");
        }

        [Test]
        public void should_get_and_set_array_element_value()
        {
            var element = JElement.Create(ElementType.Array).AddArrayElement(ElementType.Null);
            element.IsNull.ShouldBeTrue();
            element.Value.ShouldBeNull();
            element.Value = "hai";
            element.IsString.ShouldBeTrue();
            element.Value.ShouldEqual("hai");
        }

        [Test]
        public void should_fail_to_get_and_set_object_value()
        {
            var @object = JElement.Create(ElementType.Object);
            @object.IsObject.ShouldBeTrue();
            @object.IsValue.ShouldBeFalse();
            Assert.Throws<JsonValueNotSupportedException>(() => { var value = @object.Value; });
            Assert.Throws<JsonValueNotSupportedException>(() => @object.Value = "yada");
        }

        [Test]
        public void should_fail_to_get_and_set_array_value()
        {
            var array = JElement.Create(ElementType.Array);
            array.IsArray.ShouldBeTrue();
            array.IsValue.ShouldBeFalse();
            Assert.Throws<JsonValueNotSupportedException>(() => { var value = array.Value; });
            Assert.Throws<JsonValueNotSupportedException>(() => array.Value = "yada");
        }

        // Get fields

        [Test]
        public void should_get_an_existing_field_by_name()
        {
            var element = JElement.Create(ElementType.Object);
            element.AddValueMember("field", 0);
            var child = element["field"];
            child.ShouldNotBeNull();
            child.Name.ShouldEqual("field");
            child.Value.ShouldEqual(0);
        }

        [Test]
        public void should_return_null_if_field_doesnt_exist()
        {
            JElement.Create(ElementType.Object)["field"].ShouldBeNull();
        }

        [Test]
        public void should_fail_if_trying_to_get_a_field_by_name_from_an_array()
        {
            Assert.Throws<JsonMemberNotSupportedException>(() => { 
                var field = JElement.Create(ElementType.Array)["field"]; });
        }

        [Test]
        public void should_fail_if_trying_to_get_a_field_by_name_from_a_value()
        {
            Assert.Throws<JsonMemberNotSupportedException>(() => { 
                var field = JElement.Create(ElementType.Array).AddMember("field", 0)["field"]; });
        }

        [Test]
        public void should_fail_if_trying_to_get_children_from_a_value()
        {
            Assert.Throws<JsonElementsNotSupportedException>(() => { 
                var result = JElement.Create(ElementType.Object).AddMember("field", 0).ToList(); });
        }
    }
}
