using System.IO;
using System.Text;
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
    }
}
