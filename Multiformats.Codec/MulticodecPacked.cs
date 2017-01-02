using System.Linq;
using BinaryEncoding;

namespace Multiformats.Codec
{
    public static class MulticodecPacked
    {
        public static MulticodecCode GetCode(byte[] data, int offset = 0)
        {
            if (data == null || data.Length == 0)
                return MulticodecCode.Unknown;

            ulong code;
            Binary.Varint.Read(data, offset, out code);
            return (MulticodecCode) code;
        }

        public static byte[] AddPrefix(MulticodecCode code, byte[] data, int offset = 0, int? count = null) => Binary.Varint.GetBytes((ulong) code).Concat(data.Skip(offset).Take(count ?? data.Length - offset)).ToArray();

        public static byte[] SplitPrefix(byte[] data, out MulticodecCode code) => SplitPrefix(data, 0, data.Length, out code);
        public static byte[] SplitPrefix(byte[] data, int offset, out MulticodecCode code) => SplitPrefix(data, offset, data.Length - offset, out code);

        public static byte[] SplitPrefix(byte[] data, int offset, int count, out MulticodecCode code)
        {
            ulong ulcode;
            var n = Binary.Varint.Read(data, offset, out ulcode);
            code = (MulticodecCode)ulcode;
            return data.Skip(offset + n).Take(count - (offset + n)).ToArray();
        }
    }
}
