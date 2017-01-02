using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Multiformats.Codec.Tests
{
    [TestFixture]
    public class MulticodecPackedTests
    {
        [TestCase(MulticodecCode.Unknown, "<Unknown Multicodec>")]
        [TestCase(MulticodecCode.Git, "git")]
        [TestCase(MulticodecCode.DagProtobuf, "dag-pb")]
        [TestCase(MulticodecCode.DagCBOR, "dag-cbor")]
        [TestCase(MulticodecCode.Raw, "bin")]
        [TestCase(MulticodecCode.EthereumBlock, "eth-block")]
        [TestCase(MulticodecCode.EthereumTx, "eth-tx")]
        [TestCase(MulticodecCode.BitcoinBlock, "bitcoin-block")]
        [TestCase(MulticodecCode.BitcoinTx, "bitcoin-tx")]
        [TestCase(MulticodecCode.ZcashBlock, "zcash-block")]
        [TestCase(MulticodecCode.ZcashTx, "zcash-tx")]
        public void CanGetStringValuOfCode(MulticodecCode code, string expected)
        {
            Assert.That(code.GetString(), Is.EqualTo(expected));
        }
        
        [TestCase(0UL, MulticodecCode.Unknown)]
        [TestCase(0x69UL, MulticodecCode.Git)]
        [TestCase(0x70UL, MulticodecCode.DagProtobuf)]
        [TestCase(0x71UL, MulticodecCode.DagCBOR)]
        [TestCase(0x55UL, MulticodecCode.Raw)]
        [TestCase(0x90UL, MulticodecCode.EthereumBlock)]
        [TestCase(0x91UL, MulticodecCode.EthereumTx)]
        [TestCase(0xb0UL, MulticodecCode.BitcoinBlock)]
        [TestCase(0xb1UL, MulticodecCode.BitcoinTx)]
        [TestCase(0xc0UL, MulticodecCode.ZcashBlock)]
        [TestCase(0xc1UL, MulticodecCode.ZcashTx)]
        public void CanGetCorrectEnumFromNumber(ulong n, MulticodecCode expected)
        {
            Assert.That((MulticodecCode)n, Is.EqualTo(expected));
        }

        [TestCase(MulticodecCode.Git)]
        [TestCase(MulticodecCode.DagProtobuf)]
        [TestCase(MulticodecCode.DagCBOR)]
        [TestCase(MulticodecCode.Raw)]
        [TestCase(MulticodecCode.EthereumBlock)]
        [TestCase(MulticodecCode.EthereumTx)]
        [TestCase(MulticodecCode.BitcoinBlock)]
        [TestCase(MulticodecCode.BitcoinTx)]
        [TestCase(MulticodecCode.ZcashBlock)]
        [TestCase(MulticodecCode.ZcashTx)]
        public void RoundTrip(MulticodecCode code)
        {
            var data = Encoding.UTF8.GetBytes("Hello World");
            var mcdata = MulticodecPacked.AddPrefix(code, data);
            MulticodecCode outc;
            var outdata = MulticodecPacked.SplitPrefix(mcdata, out outc);

            Assert.That(outc, Is.EqualTo(code));
            Assert.That(MulticodecPacked.GetCode(mcdata), Is.EqualTo(code));
            Assert.That(outdata, Is.EqualTo(data));
        }

        [TestCase(null)]
        [TestCase(new byte[] {})]
        [TestCase(new byte[] {255,255})]
        public void GivenInvalidCode_ReturnsUnknown(byte[] data)
        {
            var c = MulticodecPacked.GetCode(data);

            Assert.That(c, Is.EqualTo(MulticodecCode.Unknown));
        }
    }
}
