using System.Text;
using NUnit.Framework;

namespace Multiformats.Codec.Tests
{
    public class MulticodecPackedTests
    {
        [Theory]
        [TestCase(MulticodecCode.Unknown, "<Unknown Multicodec>")]
        [TestCase(MulticodecCode.GitRaw, "git-raw")]
        [TestCase(MulticodecCode.MerkleDAGProtobuf, "dag-pb")]
        [TestCase(MulticodecCode.MerkleDAGCBOR, "dag-cbor")]
        [TestCase(MulticodecCode.Raw, "raw")]
        [TestCase(MulticodecCode.EthereumBlock, "eth-block")]
        [TestCase(MulticodecCode.EthereumTransaction, "eth-tx")]
        [TestCase(MulticodecCode.BitcoinBlock, "bitcoin-block")]
        [TestCase(MulticodecCode.BitcoinTransaction, "bitcoin-tx")]
        [TestCase(MulticodecCode.ZcashBlock, "zcash-block")]
        [TestCase(MulticodecCode.ZcashTransaction, "zcash-tx")]
        [TestCase(MulticodecCode.LIB2P, "libp2p-key")]
        public void CanGetStringValuOfCode(MulticodecCode code, string expected)
        {
            Assert.AreEqual(expected,code.GetString());
        }

        [Theory]
        [TestCase(0UL, MulticodecCode.Unknown)]
        [TestCase(0x78UL, MulticodecCode.GitRaw)]
        [TestCase(0x70UL, MulticodecCode.MerkleDAGProtobuf)]
        [TestCase(0x71UL, MulticodecCode.MerkleDAGCBOR)]
        [TestCase(0x55UL, MulticodecCode.Raw)]
        [TestCase(0x90UL, MulticodecCode.EthereumBlock)]
        [TestCase(0x93UL, MulticodecCode.EthereumTransaction)]
        [TestCase(0xb0UL, MulticodecCode.BitcoinBlock)]
        [TestCase(0xb1UL, MulticodecCode.BitcoinTransaction)]
        [TestCase(0xc0UL, MulticodecCode.ZcashBlock)]
        [TestCase(0x72UL, MulticodecCode.LIB2P)]
        public void CanGetCorrectEnumFromNumber(ulong n, MulticodecCode expected)
        {
            Assert.AreEqual((MulticodecCode)n, expected);
        }

        [Theory]
        [TestCase(MulticodecCode.GitRaw)]
        [TestCase(MulticodecCode.MerkleDAGProtobuf)]
        [TestCase(MulticodecCode.MerkleDAGCBOR)]
        [TestCase(MulticodecCode.Raw)]
        [TestCase(MulticodecCode.EthereumBlock)]
        [TestCase(MulticodecCode.EthereumTransaction)]
        [TestCase(MulticodecCode.BitcoinBlock)]
        [TestCase(MulticodecCode.BitcoinTransaction)]
        [TestCase(MulticodecCode.ZcashBlock)]
        [TestCase(MulticodecCode.ZcashTransaction)]
        [TestCase(MulticodecCode.LIB2P)]
        public void RoundTrip(MulticodecCode code)
        {
            var data = Encoding.UTF8.GetBytes("Hello World");
            var mcdata = MulticodecPacked.AddPrefix(code, data);
            MulticodecCode outc;
            var outdata = MulticodecPacked.SplitPrefix(mcdata, out outc);

            Assert.AreEqual(outc, code);
            Assert.AreEqual(MulticodecPacked.GetCode(mcdata), code);
            Assert.AreEqual(outdata, data);
        }

        [Theory]
        [TestCase(null)]
        [TestCase(new byte[] { })]
        [TestCase(new byte[] { 255, 255 })]
        public void GivenInvalidCode_ReturnsUnknown(byte[] data)
        {
            var c = MulticodecPacked.GetCode(data);

            Assert.AreEqual(c, MulticodecCode.Unknown);
        }
    }
}
