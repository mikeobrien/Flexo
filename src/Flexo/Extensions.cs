using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Flexo
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
