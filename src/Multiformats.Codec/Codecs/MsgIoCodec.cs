using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Multiformats.Codec.Codecs
{
    public class MsgIoCodec : ICodec
    {
        public static string HeaderPath = "/msgio";
        public static byte[] HeaderBytes = Multicodec.Header(Encoding.UTF8.GetBytes(HeaderPath));

        public byte[] Header => HeaderBytes;

        private readonly bool _multicodec;

        protected MsgIoCodec(bool multicodec)
        {
            _multicodec = multicodec;
        }

        public static MsgIoCodec CreateMulticodec() => new MsgIoCodec(true);
        public static MsgIoCodec CreateCodec() => new MsgIoCodec(false);

        public ICodecEncoder Encoder(Stream stream) => new MsgIoEncoder(stream, this);

        private class MsgIoEncoder : ICodecEncoder
        {
            private readonly Stream _stream;
            private readonly MsgIoCodec _codec;

            public MsgIoEncoder(Stream stream, MsgIoCodec codec)
            {
                _stream = stream;
                _codec = codec;
            }

            public void Encode<T>(T obj)
            {
                var bytes = (byte[])(object) obj;
                if (bytes == null)
                    throw new InvalidDataException("input must be byte array");

                if (_codec._multicodec)
                    _stream.Write(_codec.Header, 0, _codec.Header.Length);

                MessageIo.WriteMessage(_stream, bytes, flush: true);
            }

            public async Task EncodeAsync<T>(T obj, CancellationToken cancellationToken)
            {
                var bytes = (byte[])(object)obj;
                if (bytes == null)
                    throw new InvalidDataException("input must be byte array");

                if (_codec._multicodec)
                    await _stream.WriteAsync(_codec.Header, 0, _codec.Header.Length, cancellationToken);

                await MessageIo.WriteMessageAsync(_stream, bytes, flush: true, cancellationToken: cancellationToken);
            }
        }

        public ICodecDecoder Decoder(Stream stream) => new MsgIoDecoder(stream, this);

        private class MsgIoDecoder : ICodecDecoder
        {
            private readonly Stream _stream;
            private readonly MsgIoCodec _codec;

            public MsgIoDecoder(Stream stream, MsgIoCodec codec)
            {
                _stream = stream;
                _codec = codec;
            }

            public T Decode<T>()
            {
                if (_codec._multicodec)
                    Multicodec.ConsumeHeader(_stream, _codec.Header);

                var bytes = MessageIo.ReadMessage(_stream);

                return (T)(object)bytes;
            }

            public async Task<T> DecodeAsync<T>(CancellationToken cancellationToken)
            {
                if (_codec._multicodec)
                    await Multicodec.ConsumeHeaderAsync(_stream, _codec.Header, cancellationToken);

                var bytes = await MessageIo.ReadMessageAsync(_stream, cancellationToken);

                return (T)(object)bytes;
            }
        }
    }
}
