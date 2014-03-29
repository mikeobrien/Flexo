using System.IO;
using System.Text;

namespace Flexo
{
    public interface IJsonEncoder
    {
        void Encode(JElement jsonElement, Stream stream, Encoding encoding = null, bool pretty = false);
    }

    public static class JsonEncodeExtensions
    {
        public static Stream Encode(this IJsonEncoder encoder, JElement jsonElement, Encoding encoding = null, bool pretty = false)
        {
            var stream = new MemoryStream();
            encoder.Encode(jsonElement, stream, encoding, pretty);
            stream.Position = 0;
            return stream;
        }
    }
}