using System.Threading;
using System.Threading.Tasks;

namespace Multiformats.Codec
{
    public interface ICodecEncoder
    {
        void Encode<T>(T obj);
        Task EncodeAsync<T>(T obj, CancellationToken cancellationToken);
    }
}