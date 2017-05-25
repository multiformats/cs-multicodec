using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BinaryEncoding;

namespace Multiformats.Codec
{
    public static class MessageIo
    {
        public static void WriteMessage(Stream stream, byte[] bytes, int offset = 0, int count = 0, bool flush = false)
        {
            if (count == 0)
                count = bytes.Length - offset;

            Binary.BigEndian.Write(stream, (uint)count);

            stream.Write(bytes, offset, count);
            if (flush)
                stream.Flush();
        }

        public static async Task WriteMessageAsync(Stream stream, byte[] bytes, int offset = 0, int count = 0, bool flush = false, CancellationToken? cancellationToken = null)
        {
            if (count == 0)
                count = bytes.Length - offset;

            await Binary.BigEndian.WriteAsync(stream, (uint)count);

            await stream.WriteAsync(bytes, offset, count, cancellationToken ?? CancellationToken.None);
            if (flush)
                await stream.FlushAsync(cancellationToken ?? CancellationToken.None);
        }

        public static byte[] ReadMessage(Stream stream)
        {
            var len = Binary.BigEndian.ReadUInt32(stream);
            if (len == 0)
                throw new EndOfStreamException();

            var bytes = new byte[len];
            if (stream.Read(bytes, 0, bytes.Length) != len)
                throw new Exception("Could not read full message");

            return bytes;
        }

        public static async Task<byte[]> ReadMessageAsync(Stream stream, CancellationToken cancellationToken)
        {
            var len = await Binary.BigEndian.ReadUInt32Async(stream);
            if (len == 0)
                throw new EndOfStreamException();

            var bytes = new byte[len];
            if (await stream.ReadAsync(bytes, 0, bytes.Length, cancellationToken) != len)
                throw new Exception("Could not read full message");

            return bytes;
        }
    }
}