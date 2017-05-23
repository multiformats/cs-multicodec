using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Multiformats.Codec.Codecs
{
    public class MuxCodec : ICodec
    {
        public static readonly string HeaderPath = "/multicodec";
        public static readonly byte[] HeaderBytes = Multicodec.Header(Encoding.UTF8.GetBytes(HeaderPath));

        public byte[] Header => HeaderBytes;

        private readonly ICodec[] _codecs;
        public SelectCodecDelegate Select { get; set; }
        public bool Wrap { get; set; }

        public ICodec[] Codecs => _codecs;

        public ICodec Last { get; protected set; }

        public delegate ICodec SelectCodecDelegate(object obj, ICodec[] codecs);

        protected MuxCodec(IEnumerable<ICodec> codecs, SelectCodecDelegate @select, bool wrap)
        {
            _codecs = codecs.ToArray();
            Select = @select;
            Wrap = wrap;

            Last = null;
        }

        public static MuxCodec Standard => new MuxCodec(new ICodec[]
        {
            CborCodec.CreateMulticodec(),
            JsonCodec.CreateMulticodec(false),
            JsonCodec.CreateMulticodec(true),
            ProtoBufCodec.CreateMulticodec(false),
            ProtoBufCodec.CreateMulticodec(true),
        }, SelectFirst, true);

        public static MuxCodec Create(IEnumerable<ICodec> codecs, SelectCodecDelegate @select)
            => new MuxCodec(codecs, @select ?? SelectFirst, true);

        public static ICodec CodecWithHeader(byte[] header, IEnumerable<ICodec> codecs) => codecs.SingleOrDefault(c => c.Header.SequenceEqual(header));

        private static ICodec SelectFirst(object obj, ICodec[] codecs) => codecs.First();

        private ICodec GetCodec(object obj) => Select(obj, _codecs);

        public ICodecEncoder Encoder(Stream stream) => new MuxEncoder(stream, this);

        private class MuxEncoder : ICodecEncoder
        {
            private readonly Stream _stream;
            private readonly MuxCodec _codec;

            public MuxEncoder(Stream stream, MuxCodec codec)
            {
                _stream = stream;
                _codec = codec;
            }

            public void Encode<T>(T obj)
            {
                var subcodec = _codec.GetCodec(obj);
                if (subcodec == null)
                    throw new Exception("no suitable codec found");

                if (_codec.Wrap)
                    _stream.Write(_codec.Header, 0, _codec.Header.Length);

                _codec.Last = subcodec;
                subcodec.Encoder(_stream).Encode(obj);
            }

            public async Task EncodeAsync<T>(T obj, CancellationToken cancellationToken)
            {
                var subcodec = _codec.GetCodec(obj);
                if (subcodec == null)
                    throw new Exception("no suitable codec found");

                if (_codec.Wrap)
                    await _stream.WriteAsync(_codec.Header, 0, _codec.Header.Length, cancellationToken);

                _codec.Last = subcodec;
                await subcodec.Encoder(_stream).EncodeAsync(obj, cancellationToken);
            }
        }

        public ICodecDecoder Decoder(Stream stream) => new MuxDecoder(stream, this);

        private class MuxDecoder : ICodecDecoder
        {
            private readonly Stream _stream;
            private readonly MuxCodec _codec;

            public MuxDecoder(Stream stream, MuxCodec codec)
            {
                _stream = stream;
                _codec = codec;
            }

            public T Decode<T>()
            {
                if (_codec.Wrap)
                    Multicodec.ConsumeHeader(_stream, _codec.Header);

                var hdr = Multicodec.PeekHeader(_stream);
                if (hdr == null || hdr.Length == 0)
                    throw new EndOfStreamException();

                var subcodec = _codec._codecs.SingleOrDefault(c => c.Header.SequenceEqual(hdr));
                if (subcodec == null)
                    throw new Exception($"no codec found for {Encoding.UTF8.GetString(hdr)}");

                _codec.Last = subcodec;

                return subcodec.Decoder(_stream).Decode<T>();
            }

            public async Task<T> DecodeAsync<T>(CancellationToken cancellationToken)
            {
                if (_codec.Wrap)
                    await Multicodec.ConsumeHeaderAsync(_stream, _codec.Header, cancellationToken);

                var hdr = await Multicodec.PeekHeaderAsync(_stream, cancellationToken);
                if (hdr == null || hdr.Length == 0)
                    throw new EndOfStreamException();

                var subcodec = _codec._codecs.SingleOrDefault(c => c.Header.SequenceEqual(hdr));
                if (subcodec == null)
                    throw new Exception($"no codec found for {Encoding.UTF8.GetString(hdr)}");

                _codec.Last = subcodec;

                return await subcodec.Decoder(_stream).DecodeAsync<T>(cancellationToken);
            }
        }
    }
}
