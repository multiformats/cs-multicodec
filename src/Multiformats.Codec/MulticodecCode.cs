namespace Multiformats.Codec
{
    public partial struct MulticodecCode
    {
        public MultiCodecCodeType Type { get; }
        public char? Symbol { get; }
        public ulong? Code { get; }
        public string Name { get; }
        public string Description { get; }
        public string Alias { get; }

        private MulticodecCode(MultiCodecCodeType type, string name, string description, char? symbol, ulong? code, string alias = null)
        {
            Type = type;
            Name = name;
            Description = description;
            Symbol = symbol;
            Code = code;
            Alias = alias;
        }

        public MulticodecCode(string name, string description, char? symbol = null, string alias = null)
            : this(MultiCodecCodeType.Symbol, name, description, symbol, null, alias)
        {
        }

        public MulticodecCode(string name, string description, ulong? code = null, string alias = null)
            : this(MultiCodecCodeType.Code, name, description, null, code, alias)
        {
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            var other = (MulticodecCode)obj;

            return Type == other.Type &&
                Name == other.Name &&
                Description == other.Description &&
                Symbol == other.Symbol &&
                Code == other.Code;
        }

        public override int GetHashCode() => Code.HasValue ? (int)Code.Value : Symbol.HasValue ? (int)Symbol.Value : Name.GetHashCode();
        public override string ToString() => Description;

        public static readonly MulticodecCode None = new MulticodecCode(MultiCodecCodeType.None, string.Empty, string.Empty, null, null);

        // bases
        public static readonly MulticodecCode Identity = new MulticodecCode("identity", "Raw", '\0');
        public static readonly MulticodecCode Base1 = new MulticodecCode("base1", "Unary", '1');
        public static readonly MulticodecCode Base2 = new MulticodecCode("base2", "Binary (0 and 1)", '0');
        public static readonly MulticodecCode Base8 = new MulticodecCode("base8", "Octal", '7');
        public static readonly MulticodecCode Base10 = new MulticodecCode("base10", "Decimal", '9');
        public static readonly MulticodecCode Base16 = new MulticodecCode("base16", "Hexadecimal (lower)", 'f');
        public static readonly MulticodecCode Base16Upper = new MulticodecCode("base16-upper", "Hexadecimal (upper)", 'F');
        public static readonly MulticodecCode Base32 = new MulticodecCode("base32", "Base32 (lower)", 'b');
        public static readonly MulticodecCode Base32Upper = new MulticodecCode("base32-upper", "Base32 (upper)", 'B');
        public static readonly MulticodecCode Base32Padded = new MulticodecCode("base32pad", "Base32 padded (lower)", 'c');
        public static readonly MulticodecCode Base32PaddedUpper = new MulticodecCode("base32pad-upper", "Base32 padded (upper)", 'C');
        public static readonly MulticodecCode Base32Hex = new MulticodecCode("base32hex", "Base32 hexadecimal (lower)", 'v');
        public static readonly MulticodecCode Base32HexUpper = new MulticodecCode("base32hex-upper", "Base32 hexadecimal (upper)", 'V');
        public static readonly MulticodecCode Base32HexPadded = new MulticodecCode("base32hexpad", "Base32 hexadecimal padded (lower)", 't');
        public static readonly MulticodecCode Base32HexPaddedUpper = new MulticodecCode("base32hexpad-upper", "Base32 hexadecimal padded (upper)", 'T');
        public static readonly MulticodecCode Base58Flickr = new MulticodecCode("base58flickr", "Base58 Flickr", 'Z');
        public static readonly MulticodecCode Base58Bitcoin = new MulticodecCode("base58btc", "Base58 Bitcoin", 'z');
        public static readonly MulticodecCode Base64 = new MulticodecCode("base64", "Base64", 'm');
        public static readonly MulticodecCode Base64Padded = new MulticodecCode("base64pad", "Base64 padded", 'M');
        public static readonly MulticodecCode Base64Url = new MulticodecCode("base64url", "Base64 url", 'u');
        public static readonly MulticodecCode Base64UrlPadded = new MulticodecCode("base64urlpad", "Base64 url padded", 'U');
        public static readonly MulticodecCode Base32Z = new MulticodecCode("base32z", "ZBase32", 'h');
        // missin z-base32?

        // serialization
        public static readonly MulticodecCode Raw = new MulticodecCode("raw", "Raw Binary", 0x55);
        public static readonly MulticodecCode CBOR = new MulticodecCode("cbor", "CBOR", 0x51);
        public static readonly MulticodecCode BinaryJSON = new MulticodecCode("bson", "Binary JSON", code: null);
        public static readonly MulticodecCode UniversalBinaryJSON_ = new MulticodecCode("ubjson", "Universal Binary JSON", code: null);
        public static readonly MulticodecCode ProtocolBuffers = new MulticodecCode("protobuf", "Protocol Buffers", 0x50);
        public static readonly MulticodecCode CapnProto = new MulticodecCode("capnp", "Cap-n-Proto", code: null);
        public static readonly MulticodecCode FlatBuffers = new MulticodecCode("flatbuf", "FlatBuffers", code: null);
        public static readonly MulticodecCode RecurseLengthPrefix = new MulticodecCode("rlp", "Recursive Length Prefix", 0x60);
        public static readonly MulticodecCode MessagePack = new MulticodecCode("msgpack", "MessagePack", code: null);
        public static readonly MulticodecCode Binc = new MulticodecCode("binc", "Binc", code: null);
        public static readonly MulticodecCode Bencode = new MulticodecCode("bencode", "bencode", 0x63);

        // multiformats
        public static readonly MulticodecCode Multicodec = new MulticodecCode("multicodec", "multicodec", 0x30);
        public static readonly MulticodecCode Multihash = new MulticodecCode("multihash", "multihash", 0x31);
        public static readonly MulticodecCode Multiaddress = new MulticodecCode("multiaddr", "multiaddress", 0x32);
        public static readonly MulticodecCode Multibase = new MulticodecCode("multibase", "multibase", 0x33);

        // multihashes
        public static readonly MulticodecCode MD4 = new MulticodecCode("md4", "md4", 0xd4);
        public static readonly MulticodecCode MD5 = new MulticodecCode("md5", "md5", 0xd5);
        public static readonly MulticodecCode SHA1 = new MulticodecCode("sha1", "sha1", 0x11);
        public static readonly MulticodecCode SHA2_256 = new MulticodecCode("sha2-256", "sha2-256", 0x12);
        public static readonly MulticodecCode SHA2_512_ = new MulticodecCode("sha2-512", "sha2-512", 0x13);
        public static readonly MulticodecCode DBLSHA2_256 = new MulticodecCode("dbl-sha2-256", "dbl-sha2-256", 0x56);
        public static readonly MulticodecCode SHA3_224 = new MulticodecCode("sha3-224", "sha3-224", 0x17);
        public static readonly MulticodecCode SHA3_256 = new MulticodecCode("sha3-256", "sha3-256", 0x16);
        public static readonly MulticodecCode SHA3_384 = new MulticodecCode("sha3-384", "sha3-384", 0x15);
        public static readonly MulticodecCode SHA3_512 = new MulticodecCode("sha3-512", "sha3-512", 0x14);
        public static readonly MulticodecCode SHAKE_128 = new MulticodecCode("shake-128", "shake-128", 0x18);
        public static readonly MulticodecCode SHAKE_256 = new MulticodecCode("shake-256", "shake-256", 0x19);
        public static readonly MulticodecCode KECCAK_224 = new MulticodecCode("keccak-224", "keccak-224", 0x1A);
        public static readonly MulticodecCode KECCAK_256 = new MulticodecCode("keccak-256", "keccak-256", 0x1B);
        public static readonly MulticodecCode KECCAK_384 = new MulticodecCode("keccak-384", "keccak-384", 0x1C);
        public static readonly MulticodecCode KECCAK_512 = new MulticodecCode("keccak-512", "keccak-512", 0x1D);
        public static readonly MulticodecCode MURMUR3_128 = new MulticodecCode("murmur3-128", "murmur3-128", 0x22);
        public static readonly MulticodecCode MURMUR3_32 = new MulticodecCode("murmur3-32", "murmur3-32", 0x23);

        // multiaddrs
        public static readonly MulticodecCode IP4 = new MulticodecCode("ip4", "ip4", 0x04);
        public static readonly MulticodecCode IP6 = new MulticodecCode("ip6", "ip6", 0x29);
        public static readonly MulticodecCode IP6Zone = new MulticodecCode("ip6zone", "ip6zone", 0x2A);
        public static readonly MulticodecCode DNS = new MulticodecCode("dns", "dns", 0x35);
        public static readonly MulticodecCode DNS4 = new MulticodecCode("dns4", "dns4", 0x36);
        public static readonly MulticodecCode DNS6 = new MulticodecCode("dns6", "dns6", 0x37);
        public static readonly MulticodecCode TCP = new MulticodecCode("tcp", "tcp", 0x06);
        public static readonly MulticodecCode UDP = new MulticodecCode("udp", "udp", 0x0111);
        public static readonly MulticodecCode DCCP = new MulticodecCode("dccp", "dccp", 0x21);
        public static readonly MulticodecCode SCTP = new MulticodecCode("sctp", "sctp", 0x84);
        public static readonly MulticodecCode UDT = new MulticodecCode("udt", "udt", 0x012D);
        public static readonly MulticodecCode UTP = new MulticodecCode("utp", "utp", 0x012E);
        public static readonly MulticodecCode UNIX = new MulticodecCode("unix", "unix", 0x0190);
        public static readonly MulticodecCode P2P = new MulticodecCode("p2p", "p2p", 0x01A5, alias: "ipfs");
        public static readonly MulticodecCode HTTP = new MulticodecCode("http", "http", 0x01E0);
        public static readonly MulticodecCode HTTPS = new MulticodecCode("https", "https", 0x01BB);
        public static readonly MulticodecCode QUIC = new MulticodecCode("quic", "quic", 0x01CC);
        public static readonly MulticodecCode WS = new MulticodecCode("ws", "ws", 0x01DD);
        public static readonly MulticodecCode WSS = new MulticodecCode("wss", "wss", 0x01DE);
        public static readonly MulticodecCode ONION = new MulticodecCode("onion", "onion", 0x01BC);
        public static readonly MulticodecCode P2PWebSocketStar = new MulticodecCode("p2p-websocket-star", "p2p-websocket-star", 0x01DF);
        public static readonly MulticodecCode P2PWebRTCStar = new MulticodecCode("p2p-webrtc-star", "p2p-webrtc-star", 0x0113);
        public static readonly MulticodecCode P2PWebRTCDirect = new MulticodecCode("p2p-webrtc-direct", "p2p-webrtc-direct", 0x0114);
        public static readonly MulticodecCode P2PCircuit = new MulticodecCode("p2p-circuit", "p2p-circuit", 0x0122);

        // archiving
        public static readonly MulticodecCode Tar = new MulticodecCode("tar", "tar", code: null);
        public static readonly MulticodecCode Zip = new MulticodecCode("zip", "zip", code: null);

        // imaging
        public static readonly MulticodecCode Png = new MulticodecCode("png", "png", code: null);
        public static readonly MulticodecCode Jpg = new MulticodecCode("jpg", "jpg", code: null);

        // video
        public static readonly MulticodecCode Mp4 = new MulticodecCode("mp4", "mp4", code: null);
        public static readonly MulticodecCode Mkv = new MulticodecCode("mkv", "mkv", code: null);

        // IPLD
        public static readonly MulticodecCode GitRaw = new MulticodecCode("git-raw", "Raw Git object", 0x78);
        public static readonly MulticodecCode MerkleDAGProtobuf = new MulticodecCode("dag-pb", "MerkleDAG protobuf", 0x70);
        public static readonly MulticodecCode MerkleDAGCBOR = new MulticodecCode("dag-cbor", "MerkleDAG cbor", 0x71);
        public static readonly MulticodecCode MerkleDAGJSON = new MulticodecCode("dag-json", "MerkleDAG json", 0x129);
        public static readonly MulticodecCode EthereumBlock = new MulticodecCode("eth-block", "Ethereum Block (RLP)", 0x90);
        public static readonly MulticodecCode EthereumBlockList = new MulticodecCode("eth-block-list", "Ethereum Block List (RLP)", 0x91);
        public static readonly MulticodecCode EthereumTransactionTrie = new MulticodecCode("eth-tx-trie", "Ethereum Transaction Trie (Eth-Trie)", 0x92);
        public static readonly MulticodecCode EthereumTransaction = new MulticodecCode("eth-tx", "Ethereum Transaction (RLP)", 0x93);
        public static readonly MulticodecCode EthereumTransactionReceiptTrie = new MulticodecCode("eth-tx-receipt-trie", "Ethereum Transaction Receipt Trie (Eth-Trie)", 0x94);
        public static readonly MulticodecCode EthereumTransactionReceipt = new MulticodecCode("eth-tx-receipt", "Ethereum Transaction Receipt (RLP)", 0x95);
        public static readonly MulticodecCode EthereumStateTrie = new MulticodecCode("eth-state-trie", "Ethereum State Trie (Eth-Secure-Trie)", 0x96);
        public static readonly MulticodecCode EthereumAccountSnapshot = new MulticodecCode("eth-account-snapshot", "Ethereum Account Snapshot (RLP)", 0x97);
        public static readonly MulticodecCode EthereumStorageTrie = new MulticodecCode("eth-storage-trie", "Ethereum Contract Storage Trie (Eth-Secure-Trie)", 0x98);
        public static readonly MulticodecCode BitcoinBlock = new MulticodecCode("bitcoin-block", "Bitcoin Block", 0xb0);
        public static readonly MulticodecCode BitcoinTransaction = new MulticodecCode("bitcoin-tx", "Bitcoin Tx", 0xb1);
        public static readonly MulticodecCode ZcashBlock = new MulticodecCode("zcash-block", "Zcash Block", 0xc0);
        public static readonly MulticodecCode ZcashTransaction = new MulticodecCode("zcash-tx", "Zcash Tx", 0xc1);
        public static readonly MulticodecCode StellarBlock = new MulticodecCode("stellar-block", "Stellar Block", 0xd0);
        public static readonly MulticodecCode StellarTransaction = new MulticodecCode("stellar-tx", "Stellar Tx", 0xd1);
        public static readonly MulticodecCode DecredBlock = new MulticodecCode("decred-block", "Decred Block", 0xe0);
        public static readonly MulticodecCode DecredTransaction = new MulticodecCode("decred-tx", "Decred Tx", 0xe1);
        public static readonly MulticodecCode TorrentInfo = new MulticodecCode("torrent-info", "Torrent file info field (bencoded)", 0x7b);
        public static readonly MulticodecCode TorrentFile = new MulticodecCode("torrent-file", "Torrent file (bencoded)", 0x7c);
        public static readonly MulticodecCode Ed25519PublicKey = new MulticodecCode("ed25519-pub", "Ed25519 public key", 0xed);
    }

    public enum MultiCodecCodeType
    {
        None,
        Code,
        Symbol
    }
}