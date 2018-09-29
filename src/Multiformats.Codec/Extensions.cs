using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Multiformats.Codec
{
    internal static class Extensions
    {
        public static T[] Slice<T>(this T[] array, int offset, int? count = null)
        {
            var result = new T[count ?? array.Length - offset];
            Array.Copy(array, offset, result, 0, result.Length);
            return result;
        }

        public static async Task<byte> ReadByteAsync(this Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            var buffer = new byte[1];
            return await stream.ReadAsync(buffer, 0, 1, cancellationToken) == 1 ? buffer[0] : (byte)0;
        }
    }
}