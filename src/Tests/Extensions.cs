using System.IO;
using System.Text;

namespace Tests
{
    public static class Extensions
    {
        public static Stream ToStream(this string value, Encoding encoding = null)
        {
            return new MemoryStream((encoding ?? Encoding.UTF8).GetBytes(value));
        }
    }
}
