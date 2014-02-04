using System;

namespace Flexo
{
    public class JsonException : Exception
    {
        public JsonException(string message, params object[] args) :
            base(message.ToFormat(args)) { }

        public JsonException(Exception innerException) :
            base(innerException.Message, innerException) { }

        public JsonException(Exception innerException, string message, params object[] args) : 
            base(message.ToFormat(args), innerException) { }
    }

    public class JsonNameNotSupportedException : JsonException
    {
        public JsonNameNotSupportedException() : 
            base("The root element and array elements are not named.") { }
    }

    public class JsonValueNotSupportedException : JsonException
    {
        public JsonValueNotSupportedException(ElementType type) :
            base("'{0}' elements do not have a value.", type) { }
    }

    public class JsonElementsNotSupportedException : JsonException
    {
        public JsonElementsNotSupportedException() :
            base("Values do not have child elements.") { }
    }

    public class JsonMemberNotSupportedException : JsonException
    {
        public JsonMemberNotSupportedException(ElementType type) :
            base("Members are not supported by '{0}' elements.", type) { }
    }

    public class JsonArrayElementNotSupportedException : JsonException
    {
        public JsonArrayElementNotSupportedException(ElementType type) :
            base("Array elements are not supported by '{0}' elements.", type) { }
    }

    public class JsonParseException : JsonException
    {
        public JsonParseException(string message) : base(message) { }
        public JsonParseException(Exception exception) : base(exception) { }
    }

    public class JsonEncodeException : JsonException
    {
        public JsonEncodeException(Exception exception) : base(exception) { }
    }
}
