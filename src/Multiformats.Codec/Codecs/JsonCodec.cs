using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Multiformats.Codec.Codecs
{
    public class JsonCodec : ICodec
    {
        public static readonly string HeaderPath = "/json";
        public static readonly string HeaderMsgioPath = "/json/msgio";
        public static readonly byte[] HeaderBytes = Multicodec.Header(Encoding.UTF8.GetBytes(HeaderPath));
        public static readonly byte[] HeaderMsgioBytes = Multicodec.Header(Encoding.UTF8.GetBytes(HeaderMsgioPath));

        public byte[] Header => _msgio ? HeaderMsgioBytes : HeaderBytes;

        private readonly bool _multicodec;
        private readonly bool _msgio;

        protected JsonCodec(bool multicodec, bool msgio)
        {
            _multicodec = multicodec;
            _msgio = msgio;
        }

        public static JsonCodec CreateMulticodec(bool msgio) => new JsonCodec(true, msgio);
        public static JsonCodec CreateCodec(bool msgio) => new JsonCodec(false, msgio);

        public ICodecEncoder Encoder(Stream stream) => new JsonEncoder(stream, this);

        private class JsonEncoder : ICodecEncoder
        {
            private readonly Stream _stream;
            private readonly JsonCodec _codec;

            public JsonEncoder(Stream stream, JsonCodec codec)
            {
                _stream = stream;
                _codec = codec;
            }

            public void Encode<T>(T obj)
            {
                if (_codec._multicodec)
                    _stream.Write(_codec.Header, 0, _codec.Header.Length);

                if (_codec._msgio)
                {
                    MessageIo.WriteMessage(_stream, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, Formatting.None)));
                }
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, Formatting.None) + '\n');
                    _stream.Write(bytes, 0, bytes.Length);
                }
            }

            public async Task EncodeAsync<T>(T obj, CancellationToken cancellationToken)
            {
                if (_codec._multicodec)
                    await _stream.WriteAsync(_codec.Header, 0, _codec.Header.Length, cancellationToken);

                if (_codec._msgio)
                {
                    await MessageIo.WriteMessageAsync(_stream, Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, Formatting.None)), cancellationToken: cancellationToken);
                }
                else
                {
                    var bytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj, Formatting.None) + '\n');
                    await _stream.WriteAsync(bytes, 0, bytes.Length, cancellationToken);
                }
            }
        }

        public ICodecDecoder Decoder(Stream stream) => new JsonDecoder(stream, this);

        private class JsonDecoder : ICodecDecoder
        {
            private readonly Stream _stream;
            private readonly JsonCodec _codec;

            public JsonDecoder(Stream stream, JsonCodec codec)
            {
                _stream = stream;
                _codec = codec;
            }

            public T Decode<T>()
            {
                if (_codec._multicodec)
                    Multicodec.ConsumeHeader(_stream, _codec.Header);

                var json = string.Empty;
                if (_codec._msgio)
                {
                    var bytes = MessageIo.ReadMessage(_stream);
                    json = Encoding.UTF8.GetString(bytes);
                }
                else
                {
                    json = ReadLine(_stream);
                }

                return JsonConvert.DeserializeObject<T>(json);

            }

            public async Task<T> DecodeAsync<T>(CancellationToken cancellationToken)
            {
                if (_codec._multicodec)
                    await Multicodec.ConsumeHeaderAsync(_stream, _codec.Header, cancellationToken);

                var json = string.Empty;
                if (_codec._msgio)
                {
                    var bytes = await MessageIo.ReadMessageAsync(_stream, cancellationToken);
                    json = Encoding.UTF8.GetString(bytes);
                }
                else
                {
                    json = await ReadLineAsync(_stream, cancellationToken);
                }

                return JsonConvert.DeserializeObject<T>(json);
            }

            private static string ReadLine(Stream stream)
            {
                var n = 0;
                var buffer = new byte[4096];
                var offset = 0;
                while ((n = stream.Read(buffer, offset, 1)) != -1 && buffer[offset] != Multicodec.NewLine)
                {
                    offset++;
                }
                return Encoding.UTF8.GetString(buffer.Slice(0, offset)).Trim();
            }

            private static async Task<string> ReadLineAsync(Stream stream, CancellationToken cancellationToken)
            {
                var n = 0;
                var buffer = new byte[4096];
                var offset = 0;
                while ((n = await stream.ReadAsync(buffer, offset, 1, cancellationToken)) != -1 && buffer[offset] != Multicodec.NewLine)
                {
                    offset++;
                }
                return Encoding.UTF8.GetString(buffer.Slice(0, offset)).Trim();
            }
        }
    }
}