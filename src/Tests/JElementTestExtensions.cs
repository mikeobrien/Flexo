using System;
using Flexo;
using Should;

namespace Tests
{
    public static class JElementTestExtensions
    {
        public static JElement ShouldBeRoot(this JElement element)
        {
            element.IsRoot.ShouldBeTrue();
            element.HasParent.ShouldBeFalse();
            element.Parent.ShouldBeNull();
            element.IsNamed.ShouldBeFalse();
            element.Type.ShouldNotEqual(ElementType.Boolean);
            element.Type.ShouldNotEqual(ElementType.Null);
            element.Type.ShouldNotEqual(ElementType.Number);
            element.Type.ShouldNotEqual(ElementType.String);
            return element;
        }
        public static JElement ShouldNotBeRoot(this JElement element)
        {
            element.IsRoot.ShouldBeFalse();
            element.HasParent.ShouldBeTrue();
            element.Parent.ShouldNotBeNull();
            return element;
        }

        public static JElement ShouldBeAJsonObject(this JElement element)
        {
            element.IsObject.ShouldBeTrue();
            element.Type.ShouldEqual(ElementType.Object);

            element.IsValue.ShouldBeFalse();
            element.IsArray.ShouldBeFalse();
            element.IsBoolean.ShouldBeFalse();
            element.IsNull.ShouldBeFalse();
            element.IsNumber.ShouldBeFalse();
            element.IsString.ShouldBeFalse();

            element.Type.ShouldNotEqual(ElementType.Array);
            element.Type.ShouldNotEqual(ElementType.Boolean);
            element.Type.ShouldNotEqual(ElementType.Null);
            element.Type.ShouldNotEqual(ElementType.Number);
            element.Type.ShouldNotEqual(ElementType.String);

            return element;
        }

        public static JElement ShouldBeAJsonArray(this JElement element)
        {
            element.IsArray.ShouldBeTrue();
            element.Type.ShouldEqual(ElementType.Array);

            element.IsValue.ShouldBeFalse();
            element.IsObject.ShouldBeFalse();
            element.IsBoolean.ShouldBeFalse();
            element.IsNull.ShouldBeFalse();
            element.IsNumber.ShouldBeFalse();
            element.IsString.ShouldBeFalse();

            element.Type.ShouldNotEqual(ElementType.Object);
            element.Type.ShouldNotEqual(ElementType.Boolean);
            element.Type.ShouldNotEqual(ElementType.Null);
            element.Type.ShouldNotEqual(ElementType.Number);
            element.Type.ShouldNotEqual(ElementType.String);

            return element;
        }

        public static JElement ShouldBeAJsonArrayElement(this JElement element)
        {
            element.IsArrayElement.ShouldBeTrue();
            element.IsField.ShouldBeFalse();
            element.Parent.IsArray.ShouldBeTrue();
            element.Parent.IsObject.ShouldBeFalse();
            element.IsNamed.ShouldBeFalse();
            return element;
        }

        public static JElement ShouldBeAJsonArrayField(this JElement element, string name)
        {
            return element.ShouldBeAJsonArray().ShouldBeAJsonField(name);
        }

        public static JElement ShouldBeAJsonArrayArrayElement(this JElement element)
        {
            return element.ShouldBeAJsonArray().ShouldBeAJsonArrayElement();
        }         

        public static JElement ShouldBeAJsonField(this JElement element, string name)
        {
            element.IsField.ShouldBeTrue();
            element.IsArrayElement.ShouldBeFalse();
            element.Parent.IsArray.ShouldBeFalse();
            element.Parent.IsObject.ShouldBeTrue();
            element.IsNamed.ShouldBeTrue();
            element.Name.ShouldEqual(name);
            return element;
        }

        public static JElement ShouldBeAJsonObjectField(this JElement element, string name)
        {
            return element.ShouldBeAJsonObject().ShouldBeAJsonField(name);
        }

        public static JElement ShouldBeAJsonObjectArrayElement(this JElement element)
        {
            return element.ShouldBeAJsonObject().ShouldBeAJsonArrayElement();
        }

        public static JElement ShouldNotBeAJsonValue(this JElement element)
        {
            element.IsValue.ShouldBeFalse();
            element.IsBoolean.ShouldBeFalse();
            element.IsNull.ShouldBeFalse();
            element.IsNumber.ShouldBeFalse();
            element.IsString.ShouldBeFalse();

            element.Type.ShouldNotEqual(ElementType.Null);
            element.Type.ShouldNotEqual(ElementType.String);
            element.Type.ShouldNotEqual(ElementType.Number);
            element.Type.ShouldNotEqual(ElementType.Boolean);

            return element;
        }

        public static JElement ShouldBeAJsonValue(this JElement element)
        {
            element.ShouldNotBeRoot();

            element.IsValue.ShouldBeTrue();

            element.Type.ShouldNotEqual(ElementType.Object);
            element.Type.ShouldNotEqual(ElementType.Array);

            element.IsObject.ShouldBeFalse();
            element.IsArray.ShouldBeFalse();

            return element;
        }

        public static JElement ShouldBeAJsonNullValueArrayElement(this JElement element)
        {
            return element.ShouldBeAJsonValueArrayElement(ElementType.Null, null);
        }

        public static JElement ShouldBeAJsonStringValueArrayElement(this JElement element, string value = null)
        {
            return element.ShouldBeAJsonValueArrayElement(ElementType.String, value);
        }

        public static JElement ShouldBeAJsonBoolValueArrayElement(this JElement element, bool? value = null)
        {
            return element.ShouldBeAJsonValueArrayElement(ElementType.Boolean, value);
        }

        public static JElement ShouldBeAJsonNumberValueArrayElement(this JElement element, decimal? value = null)
        {
            return element.ShouldBeAJsonValueArrayElement(ElementType.Number, value);
        }

        public static JElement ShouldBeAJsonValueArrayElement(this JElement element, ElementType type, object value)
        {
            return element.ShouldBeAJsonArrayElement().ShouldBeAJsonValue(type, value);
        }

        public static JElement ShouldBeAJsonNullValueField(this JElement element, string name)
        {
            return element.ShouldBeAJsonValueField(ElementType.Null, name);
        }

        public static JElement ShouldBeAJsonStringValueField(this JElement element, string name, string value = null)
        {
            return element.ShouldBeAJsonValueField(ElementType.String, name, value);
        }

        public static JElement ShouldBeAJsonBoolValueField(this JElement element, string name, bool? value = null)
        {
            return element.ShouldBeAJsonValueField(ElementType.Boolean, name, value);
        }

        public static JElement ShouldBeAJsonNumberValueField(this JElement element, string name, decimal? value = null)
        {
            return element.ShouldBeAJsonValueField(ElementType.Number, name, value);
        }

        public static JElement ShouldBeAJsonValueField(this JElement element, ElementType type, string name, object value = null)
        {
            return element.ShouldBeAJsonField(name).ShouldBeAJsonValue(type, value);
        }

        public static JElement ShouldBeAJsonValue(this JElement element, ElementType type, object value = null)
        {
            element.ShouldBeAJsonValue();

            if (type == ElementType.Null)
            {
                type.ShouldEqual(ElementType.Null);
                element.Value.ShouldBeNull();
            } 
            else type.ShouldNotEqual(ElementType.Null);

            if (type == ElementType.String)
            {
                type.ShouldEqual(ElementType.String);
                element.Value.ShouldEqual(value);
            } 
            else type.ShouldNotEqual(ElementType.String);

            if (type == ElementType.Number)
            {
                type.ShouldEqual(ElementType.Number);
                Convert.ToDecimal(element.Value).ShouldEqual(Convert.ToDecimal(value));
            }
            else type.ShouldNotEqual(ElementType.Number);

            if (type == ElementType.Boolean)
            {
                type.ShouldEqual(ElementType.Boolean);
                element.Value.ShouldEqual(value);
            }
            else type.ShouldNotEqual(ElementType.Boolean);
            
            return element;
        }
    }
}