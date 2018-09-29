using System.Text;
using Xunit;

namespace Multiformats.Codec.Tests
{
    public class MulticodecPackedTests
    {
        [Theory]
        [InlineData(MulticodecCode.Unknown, "<Unknown Multicodec>")]
        [InlineData(MulticodecCode.GitRaw, "git-raw")]
        [InlineData(MulticodecCode.MerkleDAGProtobuf, "dag-pb")]
        [InlineData(MulticodecCode.MerkleDAGCBOR, "dag-cbor")]
        [InlineData(MulticodecCode.Raw, "raw")]
        [InlineData(MulticodecCode.EthereumBlock, "eth-block")]
        [InlineData(MulticodecCode.EthereumTransaction, "eth-tx")]
        [InlineData(MulticodecCode.BitcoinBlock, "bitcoin-block")]
        [InlineData(MulticodecCode.BitcoinTransaction, "bitcoin-tx")]
        [InlineData(MulticodecCode.ZcashBlock, "zcash-block")]
        [InlineData(MulticodecCode.ZcashTransaction, "zcash-tx")]
        public void CanGetStringValuOfCode(MulticodecCode code, string expected)
        {
            Assert.Equal(code.GetString(), expected);
        }

        [Theory]
        [InlineData(0UL, MulticodecCode.Unknown)]
        [InlineData(0x78UL, MulticodecCode.GitRaw)]
        [InlineData(0x70UL, MulticodecCode.MerkleDAGProtobuf)]
        [InlineData(0x71UL, MulticodecCode.MerkleDAGCBOR)]
        [InlineData(0x55UL, MulticodecCode.Raw)]
        [InlineData(0x90UL, MulticodecCode.EthereumBlock)]
        [InlineData(0x93UL, MulticodecCode.EthereumTransaction)]
        [InlineData(0xb0UL, MulticodecCode.BitcoinBlock)]
        [InlineData(0xb1UL, MulticodecCode.BitcoinTransaction)]
        [InlineData(0xc0UL, MulticodecCode.ZcashBlock)]
        [InlineData(0xc1UL, MulticodecCode.ZcashTransaction)]
        public void CanGetCorrectEnumFromNumber(ulong n, MulticodecCode expected)
        {
            Assert.Equal((MulticodecCode)n, expected);
        }

        [Theory]
        [InlineData(MulticodecCode.GitRaw)]
        [InlineData(MulticodecCode.MerkleDAGProtobuf)]
        [InlineData(MulticodecCode.MerkleDAGCBOR)]
        [InlineData(MulticodecCode.Raw)]
        [InlineData(MulticodecCode.EthereumBlock)]
        [InlineData(MulticodecCode.EthereumTransaction)]
        [InlineData(MulticodecCode.BitcoinBlock)]
        [InlineData(MulticodecCode.BitcoinTransaction)]
        [InlineData(MulticodecCode.ZcashBlock)]
        [InlineData(MulticodecCode.ZcashTransaction)]
        public void RoundTrip(MulticodecCode code)
        {
            var data = Encoding.UTF8.GetBytes("Hello World");
            var mcdata = MulticodecPacked.AddPrefix(code, data);
            MulticodecCode outc;
            var outdata = MulticodecPacked.SplitPrefix(mcdata, out outc);

            Assert.Equal(outc, code);
            Assert.Equal(MulticodecPacked.GetCode(mcdata), code);
            Assert.Equal(outdata, data);
        }

        [Theory]
        [InlineData(null)]
        [InlineData(new byte[] { })]
        [InlineData(new byte[] { 255, 255 })]
        public void GivenInvalidCode_ReturnsUnknown(byte[] data)
        {
            var c = MulticodecPacked.GetCode(data);

            Assert.Equal(c, MulticodecCode.Unknown);
        }
    }
}
