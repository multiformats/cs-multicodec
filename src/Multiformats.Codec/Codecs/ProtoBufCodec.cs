using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ProtoBuf;

namespace Multiformats.Codec.Codecs
{
    public class ProtoBufCodec : ICodec
    {
        public static readonly string HeaderPath = "/protobuf";
        public static readonly string HeaderMsgIoPath = "/protobuf/msgio";
        public static readonly byte[] HeaderBytes = Multicodec.Header(Encoding.UTF8.GetBytes(HeaderPath));
        public static readonly byte[] HeaderMsgIoBytes = Multicodec.Header(Encoding.UTF8.GetBytes(HeaderMsgIoPath));

        public byte[] Header => _msgio ? HeaderMsgIoBytes : HeaderBytes;

        private readonly bool _multicodec;
        private readonly bool _msgio;

        protected ProtoBufCodec(bool multicodec, bool msgio)
        {
            _multicodec = multicodec;
            _msgio = msgio;
        }

        public static ProtoBufCodec CreateMulticodec(bool msgio) => new ProtoBufCodec(true, msgio);
        public static ProtoBufCodec CreateCodec(bool msgio) => new ProtoBufCodec(false, msgio);

        public ICodecEncoder Encoder(Stream stream) => new ProtoBufEncoder(stream, this);

        private class ProtoBufEncoder : ICodecEncoder
        {
            private readonly Stream _stream;
            private readonly ProtoBufCodec _codec;

            public ProtoBufEncoder(Stream stream, ProtoBufCodec codec)
            {
                _stream = stream;
                _codec = codec;
            }

            private static byte[] Serialize<T>(T obj)
            {
                using (var stream = new MemoryStream())
                {
                    Serializer.Serialize(stream, obj);
                    return stream.ToArray();
                }
            }

            public void Encode<T>(T obj)
            {
                if (_codec._multicodec)
                    _stream.Write(_codec.Header, 0, _codec.Header.Length);

                if (_codec._msgio)
                {
                    MessageIo.WriteMessage(_stream, Serialize(obj));
                }
                else
                {
                    ProtoBuf.Serializer.SerializeWithLengthPrefix(_stream, obj, PrefixStyle.Fixed32BigEndian);
                }
                _stream.Flush();
            }

            public async Task EncodeAsync<T>(T obj, CancellationToken cancellationToken = default(CancellationToken))
            {
                if (_codec._multicodec)
                    await _stream.WriteAsync(_codec.Header, 0, _codec.Header.Length, cancellationToken);

                if (_codec._msgio)
                {
                    await MessageIo.WriteMessageAsync(_stream, Serialize(obj), cancellationToken: cancellationToken);
                }
                else
                {
                    ProtoBuf.Serializer.SerializeWithLengthPrefix(_stream, obj, PrefixStyle.Fixed32BigEndian);
                }
                await _stream.FlushAsync(cancellationToken);
            }
        }

        public ICodecDecoder Decoder(Stream stream) => new ProtoBufDecoder(stream, this);

        private class ProtoBufDecoder : ICodecDecoder
        {
            private readonly Stream _stream;
            private readonly ProtoBufCodec _codec;

            public ProtoBufDecoder(Stream stream, ProtoBufCodec codec)
            {
                _stream = stream;
                _codec = codec;
            }

            private static T Deserialize<T>(byte[] buffer)
            {
                using (var stream = new MemoryStream(buffer))
                {
                    return Serializer.Deserialize<T>(stream);
                }
            }
            
            public T Decode<T>()
            {
                if (_codec._multicodec)
                    Multicodec.ConsumeHeader(_stream, _codec.Header);

                if (_codec._msgio)
                    return Deserialize<T>(MessageIo.ReadMessage(_stream));

                return ProtoBuf.Serializer.DeserializeWithLengthPrefix<T>(_stream, PrefixStyle.Fixed32BigEndian);
            }

            public async Task<T> DecodeAsync<T>(CancellationToken cancellationToken = default(CancellationToken))
            {
                if (_codec._multicodec)
                    await Multicodec.ConsumeHeaderAsync(_stream, _codec.Header, cancellationToken);

                if (_codec._msgio)
                    return Deserialize<T>(await MessageIo.ReadMessageAsync(_stream, cancellationToken));

                return ProtoBuf.Serializer.DeserializeWithLengthPrefix<T>(_stream, PrefixStyle.Fixed32BigEndian);
            }
        }
    }
}
