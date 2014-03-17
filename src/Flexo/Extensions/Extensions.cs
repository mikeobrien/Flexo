using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Flexo.Extensions
{
    public static class Extensions
    {
        public static string ToFormat(this string value, params object[] args)
        {
            return args.Any() ? string.Format(value, args) : value;
        }

        public static T AddItem<T>(this IList<T> list, T item)
        {
            list.Add(item);
            return item;
        }

        public static bool IsNumeric(this object value)
        {
            return value is decimal || value is float || value is double || value is sbyte || value is byte ||
                   value is short || value is ushort || value is int || value is uint || value is long || value is ulong;
        }

        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        public static XElement CreateElement(this XElement element, string name)
        {
            var newElement = new XElement(name);
            element.Add(newElement);
            return newElement;
        }

        public static string GetPath(this XElement element)
        {
            var ancestors = element.Ancestors().ToList();
            return (ancestors.Any() ? "/" + ancestors.Select(x => x.Name.LocalName).Reverse()
                .Aggregate((a, i) => a + "/" + i) : "") + "/" + element.Name.LocalName;
        }

        public static IEnumerable<T> Walk<T>(this T source, Func<T, T> map) where T : class
        {
            var current = source;
            while (current != null)
            {
                yield return current;
                current = map(current);
            }
        }

        public static string Aggregate(this IEnumerable<string> source)
        {
            return source.Aggregate((a, i) => a + i);
        }

        // http://www.w3.org/TR/REC-xml/#d0e804

        private const string StartChars =
            ":A-Z_a-z" + 
            "\\u00C0-\\u00D6\\u00D8-\\u00F6\\u00F8-\\u02FF\\u0370-\\u037D" + 
            "\\u037F-\\u1FFF\\u200C-\\u200D\\u2070-\\u218F\\u2C00-\\u2FEF" + 
            "\\u3001-\\uD7FF\\uF900-\\uFDCF\\uFDF0-\\uFFFD";
        private const string BodyChars = StartChars + "\\-\\.0-9\\u00B7\\u0300-\\u036F\\u203F-\\u2040";
        private const string ValidXmlName = "^[" + StartChars + "][" + BodyChars + "]*$";
        private static readonly Regex XmlNameRegex = new Regex(ValidXmlName);

        public static bool IsValidXmlName(this string name)
        {
            return XmlNameRegex.IsMatch(name);
        }
    }

    public static class Exception<TSource> where TSource : Exception
    {
        public static TReturn Map<TReturn, TTarget>(Func<TReturn> func, Func<TSource, TTarget> map)
            where TTarget : Exception
        {
            try
            {
                return func();
            }
            catch (TSource exception)
            {
                throw map(exception);
            }
        }
    }
}
