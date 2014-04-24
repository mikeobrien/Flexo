using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Flexo.Extensions;

namespace Flexo
{
    public enum ElementType { Null, String, Number, Boolean, Array, Object }
    public enum ValueElementType { String, Number, Boolean }

    public class JElement : IEnumerable<JElement>
    {
        private static readonly IJsonParser Parser = new XmlJsonParser();
        private static readonly IJsonEncoder Encoder = new XmlJsonEncoder();

        private readonly List<JElement> _elements = new List<JElement>();
        private string _name;
        private object _value;

        public JElement(ElementType type)
        {
            Type = type;
        }

        public JElement(string name, ElementType type)
        {

            _name = name;
            Type = type;
        }

        public JElement(object value, ValueElementType type)
        {
            _value = value;
            Type = type.ToElementType();
        }

        public JElement(string name, object value, ValueElementType type)
        {
            _name = name;
            _value = value;
            Type = type.ToElementType();
        }

        private JElement(JElement parent, ElementType type)
        {
            Parent = parent;
            Type = type;
            _value = GetDefaultValue(type);
        }

        private JElement(JElement parent, object value)
        {
            Parent = parent;
            Value = value;
        }

        private JElement(JElement parent, ElementType type, string name) 
            : this(parent, type)
        {
            Name = name;
        }

        private JElement(JElement parent, string name, object value)
            : this(parent, value)
        {
            Name = name;
        }

        public static JElement Create(ElementType type)
        {
            return new JElement(type);
        }

        public static JElement Load(string json)
        {
            return Load(Encoding.UTF8.GetBytes(json));
        }

        public static JElement Load(byte[] bytes, Encoding encoding = null)
        {
            return Parser.Parse(new MemoryStream(bytes), encoding);
        }

        public static JElement Load(Stream stream, Encoding encoding = null)
        {
            return Parser.Parse(stream, encoding);
        }

        public ElementType Type { get; set; }

        public JElement Parent { get; protected set; }
        public bool HasParent { get { return Parent != null; } }
        public bool IsRoot { get { return !HasParent; } }

        public bool IsObject { get { return Type == ElementType.Object; } }
        public bool IsField { get { return HasParent && Parent.IsObject; } }

        public bool IsArray { get { return Type == ElementType.Array; } }
        public bool IsArrayElement { get { return HasParent && Parent.IsArray; } }

        public bool IsNamed { get { return (HasParent && Parent.IsObject) || !string.IsNullOrEmpty(_name); } }

        public bool IsValue { get { return !IsObject && !IsArray; } }
        public bool IsNull { get { return IsValue && Type == ElementType.Null; } }
        public bool IsString { get { return IsValue && Type == ElementType.String; } }
        public bool IsNumber { get { return IsValue && Type == ElementType.Number; } }
        public bool IsBoolean { get { return IsValue && Type == ElementType.Boolean; } }

        public string Name
        {
            get
            {
                if (!IsNamed) throw new JsonNameNotSupportedException();
                return _name;
            }
            set
            {
                if (!IsNamed) throw new JsonNameNotSupportedException();
                _name = value;
            }
        }

        public object Value
        {
            get
            {
                if (!IsValue) throw new JsonValueNotSupportedException(Type);
                return _value;
            }
            set
            {
                if (!IsValue) throw new JsonValueNotSupportedException(Type);
                _value = value;
                if (value == null) Type = ElementType.Null;
                else if (value is bool) Type = ElementType.Boolean;
                else if (value.IsNumeric()) Type = ElementType.Number;
                else Type = ElementType.String;
            }
        }

        public string Path
        {
            get
            {
                return this.Walk(x => x.Parent).Reverse().Select(x =>
                {
                    if (x.IsRoot) return "$";
                    if (x.IsNamed) return "." + x.Name;
                    return "[" + (x.Parent.ToList().IndexOf(x) + 1) + "]";
                }).Aggregate();
            }
        }

        public JElement this[string name]
        {
            get
            {
                if (!IsObject) throw new JsonMemberNotSupportedException(Type);
                return this.FirstOrDefault(x => x.Name == name);
            }
        }

        public bool CanInsert(JElement element)
        {
            return !IsValue && (IsArray || (IsObject && element.IsNamed));
        }

        public JElement Insert(JElement element)
        {
            if (IsValue) throw new JsonElementsNotSupportedException();
            if (IsObject && !element.IsNamed) throw new JsonUnnamedElementsNotSupportedException();
            if (IsArray && element.IsNamed) element.Name = null;
            element.Parent = this;
            return _elements.AddItem(element);
        }

        public JElement AddMember(string name, ElementType type)
        {
            if (!IsObject) throw new JsonMemberNotSupportedException(Type);
            return _elements.AddItem(new JElement(this, type, name));
        }

        public JElement AddValueMember(string name, object value)
        {
            if (!IsObject) throw new JsonMemberNotSupportedException(Type);
            return _elements.AddItem(new JElement(this, name, value));
        }

        public JElement AddArrayElement(ElementType type)
        {
            if (!IsArray) throw new JsonArrayElementNotSupportedException(Type);
            return _elements.AddItem(new JElement(this, type));
        }

        public JElement AddArrayValueElement(object value)
        {
            if (!IsArray) throw new JsonArrayElementNotSupportedException(Type);
            return _elements.AddItem(new JElement(this, value));
        }

        public Stream Encode(bool pretty = false)
        {
            return Encode(Encoding.UTF8, pretty);
        }

        public Stream Encode(Encoding encoding, bool pretty = false)
        {
            return Encoder.Encode(this, encoding, pretty);
        }

        public void Encode(Stream stream, bool pretty = false)
        {
            Encoder.Encode(this, stream, Encoding.UTF8, pretty);
        }

        public void Encode(Stream stream, Encoding encoding, bool pretty = false)
        {
            Encoder.Encode(this, stream, encoding, pretty);
        }

        public override string ToString()
        {
            return ToString(Encoding.UTF8);
        }

        public string ToString(bool pretty)
        {
            return ToString(Encoding.UTF8, pretty);
        }

        public string ToString(Encoding encoding, bool pretty = false)
        {
            return new StreamReader(Encode(encoding, pretty)).ReadToEnd();
        }

        public IEnumerator<JElement> GetEnumerator()
        {
            if (IsValue) throw new JsonElementsNotSupportedException();
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private static object GetDefaultValue(ElementType type)
        {
            switch (type)
            {
                case ElementType.Boolean: return false;
                case ElementType.Number: return 0;
                case ElementType.String: return "";
                default: return null;
            }
        }
    }

    public static class Extenions
    {
        public static ElementType ToElementType(this ValueElementType type)
        {
            switch (type)
            {
                case ValueElementType.Boolean: return ElementType.Boolean;
                case ValueElementType.Number: return ElementType.Number;
                case ValueElementType.String: return ElementType.String;
                default: throw new NotSupportedException();
            }
        }
    }
}