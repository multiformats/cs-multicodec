using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Multiformats.Codec
{
    public static class Multicodec
    {
        internal const byte NewLine = (byte) '\n';

        public static byte[] Header(byte[] path)
        {
            var length = path.Length + 1;
            if (length >= 127)
                throw new Exception("Multicodec varints not supported");

            return new[] {(byte) length}.Concat(path).Concat(new[] {NewLine}).ToArray();
        }

        public static byte[] HeaderPath(byte[] header)
        {
            header = header.Slice(1);
            if (header[header.Length - 1] == NewLine)
                header = header.Slice(0, header.Length - 1);
            return header;
        }

        public static void WriteHeader(Stream stream, byte[] path)
        {
            var header = Header(path);
            stream.Write(header, 0, header.Length);
        }

        public static byte[] ReadHeader(Stream stream)
        {
            var length = stream.ReadByte();
            if (length > 127)
                throw new Exception($"[ReadHeader] Multicodec varints not supported, got {length}.");

            if (length <= 0)
                throw new Exception($"Zero or negative length: {length}");

            var buf = new byte[length + 1];
            buf[0] = (byte)length;
            if (stream.Read(buf, 1, length) != length)
                throw new Exception("Could not read header");

            if (buf[length] != NewLine)
                throw new Exception("Invalid header");

            return buf;
        }

        public static async Task<byte[]> ReadHeaderAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            var length = await stream.ReadByteAsync(cancellationToken);
            if (length > 127)
                throw new Exception($"[ReadHeader] Multicodec varints not supported, got {length}.");

            if (length <= 0)
                throw new Exception($"Zero or negative length: {length}");

            var buf = new byte[length + 1];
            buf[0] = length;
            if (await stream.ReadAsync(buf, 1, length, cancellationToken) != length)
                throw new Exception("Could not read header");

            if (buf[length] != NewLine)
                throw new Exception("Invalid header");

            return buf;
        }

        public static byte[] PeekHeader(Stream stream)
        {
            if (!stream.CanSeek)
                throw new Exception("Stream does not support peeking");

            var buf = ReadHeader(stream);
            if (buf.Length > 0)
                stream.Seek(-buf.Length, SeekOrigin.Current);

            return buf;
        }

        public static async Task<byte[]> PeekHeaderAsync(Stream stream, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!stream.CanSeek)
                throw new Exception("Stream does not support peeking");

            var buf = await ReadHeaderAsync(stream, cancellationToken);
            if (buf.Length > 0)
                stream.Seek(-buf.Length, SeekOrigin.Current);

            return buf;
        }

        public static byte[] ReadPath(Stream stream)
        {
            var header = ReadHeader(stream);
            return HeaderPath(header);
        }

        public static void ConsumePath(Stream stream, byte[] path)
        {
            var actual = ReadPath(stream);
            if (!actual.SequenceEqual(path))
                throw new Exception("Mismatch");
        }

        public static void ConsumeHeader(Stream stream, byte[] header)
        {
            var actual = new byte[header.Length];
            if (stream.Read(actual, 0, actual.Length) != actual.Length)
                throw new Exception("Could not consume header");

            if (!actual.SequenceEqual(header))
                throw new Exception("Mismatch");
        }

        public static async Task ConsumeHeaderAsync(Stream stream, byte[] header, CancellationToken cancellationToken = default(CancellationToken))
        {
            var actual = new byte[header.Length];
            if (await stream.ReadAsync(actual, 0, actual.Length, cancellationToken) != actual.Length)
                throw new Exception("Could not consume header");

            if (!actual.SequenceEqual(header))
                throw new Exception("Mismatch");
        }
    }
}