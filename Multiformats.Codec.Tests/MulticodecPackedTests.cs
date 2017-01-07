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
        [TestCase(MulticodecCode.MerkleDAGProtobuf, "dag-pb")]
        [TestCase(MulticodecCode.MerkleDAGCBOR, "dag-cbor")]
        [TestCase(MulticodecCode.Raw, "bin")]
        [TestCase(MulticodecCode.EthereumBlock, "eth-block")]
        [TestCase(MulticodecCode.EthereumTransaction, "eth-tx")]
        [TestCase(MulticodecCode.BitcoinBlock, "bitcoin-block")]
        [TestCase(MulticodecCode.BitcoinTransaction, "bitcoin-tx")]
        [TestCase(MulticodecCode.ZcashBlock, "zcash-block")]
        [TestCase(MulticodecCode.ZcashTransaction, "zcash-tx")]
        public void CanGetStringValuOfCode(MulticodecCode code, string expected)
        {
            Assert.That(code.GetString(), Is.EqualTo(expected));
        }
        
        [TestCase(0UL, MulticodecCode.Unknown)]
        [TestCase(0x69UL, MulticodecCode.Git)]
        [TestCase(0x70UL, MulticodecCode.MerkleDAGProtobuf)]
        [TestCase(0x71UL, MulticodecCode.MerkleDAGCBOR)]
        [TestCase(0x55UL, MulticodecCode.Raw)]
        [TestCase(0x90UL, MulticodecCode.EthereumBlock)]
        [TestCase(0x93UL, MulticodecCode.EthereumTransaction)]
        [TestCase(0xb0UL, MulticodecCode.BitcoinBlock)]
        [TestCase(0xb1UL, MulticodecCode.BitcoinTransaction)]
        [TestCase(0xc0UL, MulticodecCode.ZcashBlock)]
        [TestCase(0xc1UL, MulticodecCode.ZcashTransaction)]
        public void CanGetCorrectEnumFromNumber(ulong n, MulticodecCode expected)
        {
            Assert.That((MulticodecCode)n, Is.EqualTo(expected));
        }

        [TestCase(MulticodecCode.Git)]
        [TestCase(MulticodecCode.MerkleDAGProtobuf)]
        [TestCase(MulticodecCode.MerkleDAGCBOR)]
        [TestCase(MulticodecCode.Raw)]
        [TestCase(MulticodecCode.EthereumBlock)]
        [TestCase(MulticodecCode.EthereumTransaction)]
        [TestCase(MulticodecCode.BitcoinBlock)]
        [TestCase(MulticodecCode.BitcoinTransaction)]
        [TestCase(MulticodecCode.ZcashBlock)]
        [TestCase(MulticodecCode.ZcashTransaction)]
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
