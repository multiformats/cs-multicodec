using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Multiformats.Codec.Codecs;
using Xunit;

namespace Multiformats.Codec.Tests
{
    public class MulticodecTests
    {
        [Serializable, ProtoBuf.ProtoContract]
        public class TestClass
        {
            [ProtoBuf.ProtoMember(1)]
            public string HelloString { get; set; }
            [ProtoBuf.ProtoMember(2)]
            public int HelloInt { get; set; }
            [ProtoBuf.ProtoMember(3)]
            public bool HelloBool { get; set; }

            public override bool Equals(object obj)
            {
                var other = (TestClass) obj;
                if (other == null)
                    return false;

                return HelloString.Equals(other.HelloString) &&
                       HelloInt.Equals(other.HelloInt) &&
                       HelloBool.Equals(other.HelloBool);
            }
        }

        private void MulticodecRoundTrip(ICodec codec)
        {
            var test = new TestClass
            {
                HelloString = "Hello World",
                HelloInt = int.MaxValue,
                HelloBool = true
            };
            TestClass result;

            using (var stream = new MemoryStream())
            {
                codec.Encoder(stream).Encode(test);
                stream.Seek(0, SeekOrigin.Begin);
                result = codec.Decoder(stream).Decode<TestClass>();
            }

            Assert.Equal(result, test);
        }

        private async Task MulticodecRoundTripAsync(ICodec codec)
        {
            var test = new TestClass
            {
                HelloString = "Hello World",
                HelloInt = int.MaxValue,
                HelloBool = true
            };
            TestClass result;

            using (var stream = new MemoryStream())
            {
                await codec.Encoder(stream).EncodeAsync(test, CancellationToken.None);
                stream.Seek(0, SeekOrigin.Begin);
                result = await codec.Decoder(stream).DecodeAsync<TestClass>(CancellationToken.None);
            }

            Assert.Equal(result, test);
        }

        [Fact]
        public void JsonMulticodecWithMsgIoRoundTrip() => MulticodecRoundTrip(JsonCodec.CreateMulticodec(true));

        [Fact]
        public void JsonCodecWithMsgIoRoundTrip() => MulticodecRoundTrip(JsonCodec.CreateCodec(true));

        [Fact]
        public void JsonMulticodecWithoutMsgIoRoundTrip() => MulticodecRoundTrip(JsonCodec.CreateMulticodec(false));

        [Fact]
        public void JsonCodecWithoutMsgIoRoundTrip() => MulticodecRoundTrip(JsonCodec.CreateCodec(false));

        [Fact]
        public void CBORMulticodecRoundTrip() => MulticodecRoundTrip(CborCodec.CreateMulticodec());

        [Fact]
        public void CBORCodecRoundTrip() => MulticodecRoundTrip(CborCodec.CreateCodec());

        [Fact]
        public Task JsonMulticodecWithMsgIoRoundTrip_Async() => MulticodecRoundTripAsync(JsonCodec.CreateMulticodec(true));

        [Fact]
        public Task JsonCodecWithMsgIoRoundTrip_Async() => MulticodecRoundTripAsync(JsonCodec.CreateCodec(true));

        [Fact]
        public Task JsonMulticodecWithoutMsgIoRoundTrip_Async() => MulticodecRoundTripAsync(JsonCodec.CreateMulticodec(false));

        [Fact]
        public Task JsonCodecWithoutMsgIoRoundTrip_Async() => MulticodecRoundTripAsync(JsonCodec.CreateCodec(false));

        [Fact]
        public Task CBORMulticodecRoundTrip_Async() => MulticodecRoundTripAsync(CborCodec.CreateMulticodec());

        [Fact]
        public Task CBORCodecRoundTrip_Async() => MulticodecRoundTripAsync(CborCodec.CreateCodec());


        private void MulticodecRoundTripMany(ICodec codec, int count = 1000)
        {
            var tests = Enumerable.Range(0, count).Select(i => new TestClass
            {
                HelloString = "Hello World " + i,
                HelloInt = int.MaxValue,
                HelloBool = true
            }).ToArray();
            var results = new List<TestClass>();

            using (var stream = new MemoryStream())
            {
                var enc = codec.Encoder(stream);
                foreach (var test in tests)
                {
                    enc.Encode(test);
                }
                stream.Seek(0, SeekOrigin.Begin);
                var dec = codec.Decoder(stream);
                for (var i = 0; i < tests.Length; i++)
                    results.Add(dec.Decode<TestClass>());
            }

            Assert.Equal(results.ToArray(), tests);
        }

        private async Task MulticodecRoundTripManyAsync(ICodec codec, int count = 1000)
        {
            var tests = Enumerable.Range(0, count).Select(i => new TestClass
            {
                HelloString = "Hello World " + i,
                HelloInt = int.MaxValue,
                HelloBool = true
            }).ToArray();
            var results = new List<TestClass>();

            using (var stream = new MemoryStream())
            {
                var enc = codec.Encoder(stream);
                foreach (var test in tests)
                {
                    await enc.EncodeAsync(test, CancellationToken.None);
                }
                stream.Seek(0, SeekOrigin.Begin);
                var dec = codec.Decoder(stream);
                for (var i = 0; i < tests.Length; i++)
                    results.Add(await dec.DecodeAsync<TestClass>(CancellationToken.None));
            }

            Assert.Equal(results.ToArray(), tests);
        }

        private void MsgIoRoundTripMany(MsgIoCodec codec)
        {
            var count = 1000;
            var r = new Random(Environment.TickCount);
            var tests = Enumerable.Range(0, count).Select(i =>
            {
                var bytes = new byte[1024];
                r.NextBytes(bytes);
                return bytes;
            }).ToArray();
            var results = new List<byte[]>();

            using (var stream = new MemoryStream())
            {
                var enc = codec.Encoder(stream);
                foreach (var test in tests)
                {
                    enc.Encode(test);
                }
                stream.Seek(0, SeekOrigin.Begin);
                var dec = codec.Decoder(stream);
                for (var i = 0; i < tests.Length; i++)
                    results.Add(dec.Decode<byte[]>());
            }

            Assert.Equal(results.ToArray(), tests);
        }

        private async Task MsgIoRoundTripManyAsync(MsgIoCodec codec)
        {
            var count = 1000;
            var r = new Random(Environment.TickCount);
            var tests = Enumerable.Range(0, count).Select(i =>
            {
                var bytes = new byte[1024];
                r.NextBytes(bytes);
                return bytes;
            }).ToArray();
            var results = new List<byte[]>();

            using (var stream = new MemoryStream())
            {
                var enc = codec.Encoder(stream);
                foreach (var test in tests)
                {
                    await enc.EncodeAsync(test, CancellationToken.None);
                }
                stream.Seek(0, SeekOrigin.Begin);
                var dec = codec.Decoder(stream);
                for (var i = 0; i < tests.Length; i++)
                    results.Add(await dec.DecodeAsync<byte[]>(CancellationToken.None));
            }

            Assert.Equal(results.ToArray(), tests);
        }

        [Fact]
        public void JsonMulticodecWithMsgIoRoundTripMany() => MulticodecRoundTripMany(JsonCodec.CreateMulticodec(true));

        [Fact]
        public void JsonCodecWithMsgIoRoundTripMany() => MulticodecRoundTripMany(JsonCodec.CreateCodec(true));

        [Fact]
        public void JsonMulticodecWithoutMsgIoRoundTripMany() => MulticodecRoundTripMany(JsonCodec.CreateMulticodec(false));

        [Fact]
        public void JsonCodecWithoutMsgIoRoundTripMany() => MulticodecRoundTripMany(JsonCodec.CreateCodec(false));

        [Fact]
        public void ProtoBufMulticodecWithMsgIoRoundTripMany() => MulticodecRoundTripMany(ProtoBufCodec.CreateMulticodec(true));

        [Fact]
        public void ProtoBufMulticodecWithoutMsgIoRoundTripMany() => MulticodecRoundTripMany(ProtoBufCodec.CreateMulticodec(false));

        [Fact]
        public void ProtoBufCodecWithMsgIoRoundTripMany() => MulticodecRoundTripMany(ProtoBufCodec.CreateCodec(true));

        [Fact]
        public void ProtoBufCodecWithoutMsgIoRoundTripMany() => MulticodecRoundTripMany(ProtoBufCodec.CreateCodec(false));

        [Fact]
        public void CBORMulticodecRoundTripMany() => MulticodecRoundTripMany(CborCodec.CreateMulticodec());

        [Fact]
        public void CBORCodecRoundTripMany() => MulticodecRoundTripMany(CborCodec.CreateCodec());

        [Fact]
        public void MsgIoMulticodecRoundTripMany() => MsgIoRoundTripMany(MsgIoCodec.CreateMulticodec());

        [Fact]
        public void MsgIoCodecRoundTripMany() => MsgIoRoundTripMany(MsgIoCodec.CreateCodec());


        [Fact]
        public Task JsonMulticodecWithMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(JsonCodec.CreateMulticodec(true));

        [Fact]
        public Task JsonCodecWithMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(JsonCodec.CreateCodec(true));

        [Fact]
        public Task JsonMulticodecWithoutMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(JsonCodec.CreateMulticodec(false));

        [Fact]
        public Task JsonCodecWithoutMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(JsonCodec.CreateCodec(false));

        [Fact]
        public Task ProtoBufMulticodecWithMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(ProtoBufCodec.CreateMulticodec(true));

        [Fact]
        public Task ProtoBufMulticodecWithiutMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(ProtoBufCodec.CreateMulticodec(false));

        [Fact]
        public Task ProtoBufCodecWithMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(ProtoBufCodec.CreateCodec(true));

        [Fact]
        public Task ProtoBufCodecWithoutMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(ProtoBufCodec.CreateCodec(false));

        [Fact]
        public Task CBORMulticodecRoundTripMany_Async() => MulticodecRoundTripManyAsync(CborCodec.CreateMulticodec());

        [Fact]
        public Task CBORCodecRoundTripMany_Async() => MulticodecRoundTripManyAsync(CborCodec.CreateCodec());

        [Fact]
        public Task MsgIoMulticodecRoundTripMany_Async() => MsgIoRoundTripManyAsync(MsgIoCodec.CreateMulticodec());

        [Fact]
        public Task MsgIoCodecRoundTripMany_Async() => MsgIoRoundTripManyAsync(MsgIoCodec.CreateCodec());


        private MuxCodec.SelectCodecDelegate SelectRandomCodec()
        {
            bool reuse = false;
            int last = 0;
            var rand = new Random(Environment.TickCount);

            return (o, codecs) =>
            {
                if (reuse)
                    reuse = false;
                else
                {
                    reuse = true;
                    last = rand.Next(0, codecs.Length);
                }
                return codecs[last%codecs.Length];
            };
        }

        private MuxCodec RandomMux()
        {
            var c = MuxCodec.Standard;
            c.Select = SelectRandomCodec();
            return c;
        }

        [Fact]
        public void MuxCodecRoundTripsWrap()
        {
            var codec = RandomMux();
            MulticodecRoundTripMany(codec);
        }

        [Fact]
        public void MuxCodecRoundTripsNoWrap()
        {
            var codec = RandomMux();
            codec.Wrap = false;
            MulticodecRoundTripMany(codec);
        }
    }
}
