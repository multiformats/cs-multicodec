using System.Threading;
using System.Threading.Tasks;

namespace Multiformats.Codec
{
    public interface ICodecDecoder
    {
        T Decode<T>();
        Task<T> DecodeAsync<T>(CancellationToken cancellationToken = default(CancellationToken));
    }
}