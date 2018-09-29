namespace Multiformats.Codec
{
    public enum MulticodecCode : ulong
    {
        // misc
        [StringValue("<Unknown Multicodec>")]
        Unknown = 0,
        [StringValue("raw")]
        Raw = 0x55,

        // bases
        Base1 = 0x01,
        //Base2 = Raw,
        Base8 = 0x07,
        Base10 = 0x09,
        Base16,
        Base32,
        Base32Hex,
        Base58Flickr,
        [StringValue("base58btc")]
        Base58BitCoin,
        Base64,
        Base64Url,

        // serialization
        CBOR = 0x51,
        [StringValue("bjson")]
        BinaryJson,
        [StringValue("ubjson")]
        UniversalBinaryJson,
        [StringValue("protobuf")]
        ProtolBuffers = 0x50,
        [StringValue("capnp")]
        CapnProto,
        [StringValue("flatbuf")]
        FlatBuffers,
        [StringValue("rlp")]
        RecursiveLengthPrefix = 0x60,
        [StringValue("msgpack")]
        MessagePack,
        [StringValue("binc")]
        Binc,
        [StringValue("bencode")]
        Bencode = 0x63,

        // multiformats
        [StringValue("multicodec")]
        Multicodec = 0x30,
        [StringValue("multihash")]
        Multihash = 0x31,
        [StringValue("multiaddr")]
        Multiaddress = 0x32,
        [StringValue("multibase")]
        Multibase = 0x33,

        // multihashes

        // multiaddrs
        IP4 = 0x04,
        IP6 = 0x29,
        TCP = 0x06,
        UDP = 0x0111,
        DCCP = 0x21,
        SCTP = 0x84,
        UDT = 0x012D,
        UTP = 0x012E,
        IPFS = 0x2A,
        HTTP = 0x01E0,
        HTTPS = 0x01BB,
        QUIC = 0x01CC,
        WS = 0x01DD,
        ONION = 0x01BC,
        P2PCircuit = 0x0122,

        // archiving
        Tar,
        Zip,

        // imaging
        Png,
        Jpg,

        // video
        Mp4,
        Mkv,

        // IPLD
        [StringValue("git-raw")]
        GitRaw = 0x78,
        [StringValue("dag-pb")]
        MerkleDAGProtobuf = 0x70,
        [StringValue("dag-cbor")]
        MerkleDAGCBOR = 0x71,
        [StringValue("dag-json")]
        MerkleDAGJSON = 0x129,
        [StringValue("eth-block")]
        EthereumBlock = 0x90,
        [StringValue("eth-block-list")]
        EthereumBlockList = 0x91,
        [StringValue("eth-tx-trie")]
        EthereumTransactionTrie = 0x92,
        [StringValue("eth-tx")]
        EthereumTransaction = 0x93,
        [StringValue("eth-tx-receipt-trie")]
        EthereumTransactionReceiptTrie = 0x94,
        [StringValue("eth-tx-receipt")]
        EthereumTransactionReceipt = 0x95,
        [StringValue("eth-state-trie")]
        EthereumStateTrie = 0x96,
        [StringValue("eth-account-snapshot")]
        EthereumAccountSnapshot = 0x97,
        [StringValue("eth-storage-trie")]
        EthereumStorageTrie = 0x98,
        [StringValue("bitcoin-block")]
        BitcoinBlock = 0xb0,
        [StringValue("bitcoin-tx")]
        BitcoinTransaction = 0xb1,
        [StringValue("zcash-block")]
        ZcashBlock = 0xc0,
        [StringValue("zcash-tx")]
        ZcashTransaction = 0xc1,
        [StringValue("stellar-block")]
        StellarBlock = 0xd0,
        [StringValue("stellar-tx")]
        StellarTransaction = 0xd1,
        [StringValue("decred-block")]
        DecredBlock = 0xe0,
        [StringValue("decred-tx")]
        DecredTransaction = 0xe1,
        [StringValue("torrent-info")]
        TorrentInfo = 0x7b,
        [StringValue("torrent-file")]
        TorrentFile = 0x7c,
        [StringValue("ed25519-pub")]
        Ed25519PublicKey = 0xed,
    }
}