using System.Text;

namespace Flexo
{
    public interface IJsonEncoder
    {
        string Encode(JElement jsonElement, Encoding encoding = null, bool pretty = false);
    }
}