using System.IO;
using System.Text;

namespace Flexo
{
    public interface IJsonParser
    {
        JElement Parse(Stream stream, Encoding encoding = null);
    }
}