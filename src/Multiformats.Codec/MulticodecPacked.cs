using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BinaryEncoding;

namespace Multiformats.Codec
{
    public static class MulticodecPacked
    {
        private static readonly IEnumerable<MulticodecCode> _codecs;
        private static readonly IDictionary<char, MulticodecCode> _symbols;
        private static readonly IDictionary<ulong, MulticodecCode> _codes;

        static MulticodecPacked()
        {
            _codecs = typeof(MulticodecCode).GetTypeInfo()
                .DeclaredFields.Where(f => f.IsStatic && f.DeclaringType == typeof(MulticodecCode))
                .Select(f => f.GetValue(default(MulticodecCode)))
                .Cast<MulticodecCode>()
                .ToArray();

            _codes = _codecs.Where(c => c.Type == MultiCodecCodeType.Code && c.Code.HasValue)
                .ToDictionary(c => c.Code.Value);
            _symbols = _codecs.Where(c => c.Type == MultiCodecCodeType.Symbol && c.Symbol.HasValue)
                .ToDictionary(c => c.Symbol.Value);
        }

        public static MulticodecCode GetCode(byte[] data, int offset = 0)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            var n = Binary.Varint.Read(data, offset, out ulong code);
            if (n == 0)
                throw new MulticodecException("Could not read variable length prefix");

            if (!_codes.TryGetValue(code, out var result))
                throw new MulticodecException($"Unsupported codec: {code}");

            return result;
        }

        public static MulticodecCode GetCode(string data, int offset = 0)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            var code = data[offset];
            if (!_symbols.TryGetValue(code, out var result))
                throw new MulticodecException($"Unsupported codec: {code}");

            return result;
        }

        public static byte[] AddPrefix(MulticodecCode code, byte[] data, int offset = 0, int? count = null)
        {
            if (code.Type != MultiCodecCodeType.Code)
                throw new ArgumentException("Multicodec code must be a uvarint type");

            return Binary.Varint.GetBytes(code.Code.Value).Concat(data.Skip(offset).Take(count ?? data.Length - offset)).ToArray();
        }

        public static string AddPrefix(MulticodecCode code, string data, int offset = 0, int? count = null)
        {
            if (code.Type != MultiCodecCodeType.Symbol)
                throw new ArgumentException("Multicodec code must be a symbol type");

            return new string(new char[] { code.Symbol.Value }.Concat(data.Skip(offset).Take(count ?? data.Length - offset)).ToArray());
        }

        public static byte[] SplitPrefix(byte[] data, out MulticodecCode code) => SplitPrefix(data, 0, data.Length, out code);
        public static string SplitPrefix(string data, out MulticodecCode code) => SplitPrefix(data, 0, data.Length, out code);

        public static byte[] SplitPrefix(byte[] data, int offset, out MulticodecCode code) => SplitPrefix(data, offset, data.Length - offset, out code);
        public static string SplitPrefix(string data, int offset, out MulticodecCode code) => SplitPrefix(data, offset, data.Length - offset, out code);

        public static byte[] SplitPrefix(byte[] data, int offset, int count, out MulticodecCode code)
        {
            if (data == null || data.Length == 0)
                throw new ArgumentNullException(nameof(data));

            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (count < 1 || count > data.Length - offset)
                throw new ArgumentOutOfRangeException(nameof(count));

            var n = Binary.Varint.Read(data, offset, out ulong ulcode);
            if (n == 0)
                throw new MulticodecException("Could not read variable length prefix");

            if (!_codes.TryGetValue(ulcode, out code))
                throw new MulticodecException($"Unsupported codec: {ulcode}");

            return data.Skip(offset + n).Take(count - (offset + n)).ToArray();
        }

        public static string SplitPrefix(string data, int offset, int count, out MulticodecCode code)
        {
            if (string.IsNullOrEmpty(data))
                throw new ArgumentNullException(nameof(data));

            if (offset < 0 || offset > data.Length)
                throw new ArgumentOutOfRangeException(nameof(offset));

            if (count < 1 || count > data.Length - offset)
                throw new ArgumentOutOfRangeException(nameof(count));

            var chcode = data[offset];
            if (!_symbols.TryGetValue(chcode, out code))
                throw new MulticodecException($"Unsupported codec: {chcode}");

            return new string(data.Skip(offset + 1).Take(count - (offset + 1)).ToArray());
        }

        public static bool TryGet(string name, out MulticodecCode code)
        {
            code = _codecs.SingleOrDefault(c => c.Name.Equals(name) || (c.Alias?.Equals(name) ?? false));
            return code.Name.Equals(name) || (code.Alias?.Equals(name) ?? false);
        }

        public static bool TryGet(char symbol, out MulticodecCode code) => _symbols.TryGetValue(symbol, out code);
        public static bool TryGet(ulong ulcode, out MulticodecCode code) => _codes.TryGetValue(ulcode, out code);
    }
}
