using System.IO;
using System.Text;

namespace Flexo
{
    public interface IJsonEncoder
    {
        Stream Encode(JElement jsonElement, Encoding encoding = null, bool pretty = false);
    }
}