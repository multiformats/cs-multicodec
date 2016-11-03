using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Multiformats.Codec.Codecs;
using NUnit.Framework;

namespace Multiformats.Codec.Tests
{
    [TestFixture]
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

            Assert.That(result, Is.EqualTo(test));
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

            Assert.That(result, Is.EqualTo(test));
        }

        [Test]
        public void JsonMulticodecWithMsgIoRoundTrip() => MulticodecRoundTrip(JsonCodec.CreateMulticodec(true));

        [Test]
        public void JsonCodecWithMsgIoRoundTrip() => MulticodecRoundTrip(JsonCodec.CreateCodec(true));

        [Test]
        public void JsonMulticodecWithoutMsgIoRoundTrip() => MulticodecRoundTrip(JsonCodec.CreateMulticodec(false));

        [Test]
        public void JsonCodecWithoutMsgIoRoundTrip() => MulticodecRoundTrip(JsonCodec.CreateCodec(false));

        [Test]
        public void CBORMulticodecRoundTrip() => MulticodecRoundTrip(CborCodec.CreateMulticodec());

        [Test]
        public void CBORCodecRoundTrip() => MulticodecRoundTrip(CborCodec.CreateCodec());

        [Test]
        public Task JsonMulticodecWithMsgIoRoundTrip_Async() => MulticodecRoundTripAsync(JsonCodec.CreateMulticodec(true));

        [Test]
        public Task JsonCodecWithMsgIoRoundTrip_Async() => MulticodecRoundTripAsync(JsonCodec.CreateCodec(true));

        [Test]
        public Task JsonMulticodecWithoutMsgIoRoundTrip_Async() => MulticodecRoundTripAsync(JsonCodec.CreateMulticodec(false));

        [Test]
        public Task JsonCodecWithoutMsgIoRoundTrip_Async() => MulticodecRoundTripAsync(JsonCodec.CreateCodec(false));

        [Test]
        public Task CBORMulticodecRoundTrip_Async() => MulticodecRoundTripAsync(CborCodec.CreateMulticodec());

        [Test]
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

            Assert.That(results.ToArray(), Is.EqualTo(tests));
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

            Assert.That(results.ToArray(), Is.EqualTo(tests));
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

            Assert.That(results.ToArray(), Is.EqualTo(tests));
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

            Assert.That(results.ToArray(), Is.EqualTo(tests));
        }

        [Test]
        public void JsonMulticodecWithMsgIoRoundTripMany() => MulticodecRoundTripMany(JsonCodec.CreateMulticodec(true));

        [Test]
        public void JsonCodecWithMsgIoRoundTripMany() => MulticodecRoundTripMany(JsonCodec.CreateCodec(true));

        [Test]
        public void JsonMulticodecWithoutMsgIoRoundTripMany() => MulticodecRoundTripMany(JsonCodec.CreateMulticodec(false));

        [Test]
        public void JsonCodecWithoutMsgIoRoundTripMany() => MulticodecRoundTripMany(JsonCodec.CreateCodec(false));

        [Test]
        public void ProtoBufMulticodecWithMsgIoRoundTripMany() => MulticodecRoundTripMany(ProtoBufCodec.CreateMulticodec(true));

        [Test]
        public void ProtoBufMulticodecWithoutMsgIoRoundTripMany() => MulticodecRoundTripMany(ProtoBufCodec.CreateMulticodec(false));

        [Test]
        public void ProtoBufCodecWithMsgIoRoundTripMany() => MulticodecRoundTripMany(ProtoBufCodec.CreateCodec(true));

        [Test]
        public void ProtoBufCodecWithoutMsgIoRoundTripMany() => MulticodecRoundTripMany(ProtoBufCodec.CreateCodec(false));

        [Test]
        public void CBORMulticodecRoundTripMany() => MulticodecRoundTripMany(CborCodec.CreateMulticodec());

        [Test]
        public void CBORCodecRoundTripMany() => MulticodecRoundTripMany(CborCodec.CreateCodec());

        [Test]
        public void MsgIoMulticodecRoundTripMany() => MsgIoRoundTripMany(MsgIoCodec.CreateMulticodec());

        [Test]
        public void MsgIoCodecRoundTripMany() => MsgIoRoundTripMany(MsgIoCodec.CreateCodec());


        [Test]
        public Task JsonMulticodecWithMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(JsonCodec.CreateMulticodec(true));

        [Test]
        public Task JsonCodecWithMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(JsonCodec.CreateCodec(true));

        [Test]
        public Task JsonMulticodecWithoutMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(JsonCodec.CreateMulticodec(false));

        [Test]
        public Task JsonCodecWithoutMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(JsonCodec.CreateCodec(false));

        [Test]
        public Task ProtoBufMulticodecWithMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(ProtoBufCodec.CreateMulticodec(true));

        [Test]
        public Task ProtoBufMulticodecWithiutMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(ProtoBufCodec.CreateMulticodec(false));

        [Test]
        public Task ProtoBufCodecWithMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(ProtoBufCodec.CreateCodec(true));

        [Test]
        public Task ProtoBufCodecWithoutMsgIoRoundTripMany_Async() => MulticodecRoundTripManyAsync(ProtoBufCodec.CreateCodec(false));

        [Test]
        public Task CBORMulticodecRoundTripMany_Async() => MulticodecRoundTripManyAsync(CborCodec.CreateMulticodec());

        [Test]
        public Task CBORCodecRoundTripMany_Async() => MulticodecRoundTripManyAsync(CborCodec.CreateCodec());

        [Test]
        public Task MsgIoMulticodecRoundTripMany_Async() => MsgIoRoundTripManyAsync(MsgIoCodec.CreateMulticodec());

        [Test]
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

        [Test]
        public void MuxCodecRoundTripsWrap()
        {
            var codec = RandomMux();
            MulticodecRoundTripMany(codec);
        }

        [Test]
        public void MuxCodecRoundTripsNoWrap()
        {
            var codec = RandomMux();
            codec.Wrap = false;
            MulticodecRoundTripMany(codec);
        }
    }
}
