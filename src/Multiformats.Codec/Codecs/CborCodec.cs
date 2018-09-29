using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PeterO.Cbor;

namespace Multiformats.Codec.Codecs
{
    public class CborCodec : ICodec
    {
        public static readonly string HeaderPath = "/cbor";
        public static readonly byte[] HeaderBytes = Multicodec.Header(Encoding.UTF8.GetBytes(HeaderPath));

        public byte[] Header => HeaderBytes;

        private readonly bool _multicodec;

        protected CborCodec(bool multicodec)
        {
            _multicodec = multicodec;
        }

        public static CborCodec CreateMulticodec() => new CborCodec(true);
        public static CborCodec CreateCodec() => new CborCodec(false);

        public ICodecEncoder Encoder(Stream stream) => new CBOREncoder(stream, this);

        private class CBOREncoder : ICodecEncoder
        {
            private readonly Stream _stream;
            private readonly CborCodec _codec;

            public CBOREncoder(Stream stream, CborCodec codec)
            {
                _stream = stream;
                _codec = codec;
            }

            public void Encode<T>(T obj)
            {
                if (_codec._multicodec)
                    _stream.Write(_codec.Header, 0, _codec.Header.Length);

                var cbor = CBORObject.FromObject(obj);
                cbor.WriteTo(_stream);
                _stream.Flush();
            }

            public async Task EncodeAsync<T>(T obj, CancellationToken cancellationToken = default(CancellationToken))
            {
                if (cancellationToken.IsCancellationRequested)
                    return;

                if (_codec._multicodec)
                    await _stream.WriteAsync(_codec.Header, 0, _codec.Header.Length, cancellationToken);

                var cbor = CBORObject.FromObject(obj);

                cbor.WriteTo(_stream);
                await _stream.FlushAsync(cancellationToken);
            }
        }

        public ICodecDecoder Decoder(Stream stream) => new CBORDecoder(stream, this);

        private class CBORDecoder : ICodecDecoder
        {
            private readonly Stream _stream;
            private readonly CborCodec _codec;

            public CBORDecoder(Stream stream, CborCodec codec)
            {
                _stream = stream;
                _codec = codec;
            }

            public T Decode<T>()
            {
                if (_codec._multicodec)
                    Multicodec.ConsumeHeader(_stream, _codec.Header);

                return CBORObject.Read(_stream).ToObject<T>();
            }

            public async Task<T> DecodeAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
            {
                if (_codec._multicodec)
                    await Multicodec.ConsumeHeaderAsync(_stream, _codec.Header, cancellationToken);

                return CBORObject.Read(_stream).ToObject<T>();
            }
        }
    }
}