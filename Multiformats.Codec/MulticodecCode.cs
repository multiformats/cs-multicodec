namespace Multiformats.Codec
{
    public enum MulticodecCode : ulong
    {
        [StringValue("<Unknown Multicodec>")]
        Unknown = 0,
        [StringValue("git")]
        Git = 0x69,
        [StringValue("dag-pb")]
        DagProtobuf = 0x70,
        [StringValue("dag-cbor")]
        DagCBOR = 0x71,
        [StringValue("bin")]
        Raw = 0x55,

        [StringValue("eth-block")]
        EthereumBlock = 0x90,
        [StringValue("eth-tx")]
        EthereumTx = 0x91,
        [StringValue("bitcoin-block")]
        BitcoinBlock = 0xb0,
        [StringValue("bitcoin-tx")]
        BitcoinTx = 0xb1,
        [StringValue("zcash-block")]
        ZcashBlock = 0xc0,
        [StringValue("zcash-tx")]
        ZcashTx = 0xc1,
    }
}