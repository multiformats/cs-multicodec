using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Xunit;

namespace Multiformats.Codec.Tests
{
    public class MulticodecPackedTests
    {
        private static readonly IEnumerable<MulticodecCode> _codecs;

        static MulticodecPackedTests()
        {
            _codecs = typeof(MulticodecCode).GetTypeInfo()
                .DeclaredFields.Where(f => f.IsStatic && f.DeclaringType == typeof(MulticodecCode))
                .Select(f => f.GetValue(default(MulticodecCode)))
                .Cast<MulticodecCode>();
        }

        public static IEnumerable<object[]> GetCodecs() => _codecs.Select(c => new object[] { c });

        public static IEnumerable<object[]> GetCodes() => _codecs
            .Where(c => c.Type == MultiCodecCodeType.Code && c.Code.HasValue)
            .Select(c => new object[] { c });

        public static IEnumerable<object[]> GetSymbols() => _codecs
            .Where(c => c.Type == MultiCodecCodeType.Symbol && c.Symbol.HasValue)
            .Select(c => new object[] { c });

        [Theory]
        [MemberData(nameof(GetCodes), DisableDiscoveryEnumeration = false)]
        public void TestRoundTripCodes(MulticodecCode code)
        {
            Assert.Equal(MultiCodecCodeType.Code, code.Type);

            var payload = Encoding.UTF8.GetBytes("Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.");

            var prefixed = MulticodecPacked.AddPrefix(code, payload);
            Assert.NotEqual(payload, prefixed);
            var prefix = MulticodecPacked.GetCode(prefixed);
            Assert.Equal(code, prefix);

            var unprefixed = MulticodecPacked.SplitPrefix(prefixed, out var splitCode);
            Assert.Equal(code, splitCode);
            Assert.Equal(payload, unprefixed);
        }

        [Theory]
        [MemberData(nameof(GetSymbols), DisableDiscoveryEnumeration = false)]
        public void TestRoundTripSymbols(MulticodecCode code)
        {
            Assert.Equal(MultiCodecCodeType.Symbol, code.Type);

            var payload = "Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.";

            var prefixed = MulticodecPacked.AddPrefix(code, payload);
            Assert.NotEqual(payload, prefixed);
            var prefix = MulticodecPacked.GetCode(prefixed);
            Assert.Equal(code, prefix);

            var unprefixed = MulticodecPacked.SplitPrefix(prefixed, out var splitCode);
            Assert.Equal(code, splitCode);
            Assert.Equal(payload, unprefixed);
        }

        [Theory]
        [MemberData(nameof(GetSymbols), DisableDiscoveryEnumeration = false)]
        public void TestGetBySymbol(MulticodecCode code)
        {
            Assert.Equal(MultiCodecCodeType.Symbol, code.Type);
            Assert.True(MulticodecPacked.TryGet(code.Symbol.Value, out var result));
            Assert.Equal(code, result);
        }

        [Theory]
        [MemberData(nameof(GetCodes), DisableDiscoveryEnumeration = false)]
        public void TestGetByCode(MulticodecCode code)
        {
            Assert.Equal(MultiCodecCodeType.Code, code.Type);
            Assert.True(MulticodecPacked.TryGet(code.Code.Value, out var result));
            Assert.Equal(code, result);
        }

        [Theory]
        [MemberData(nameof(GetCodecs), DisableDiscoveryEnumeration = false)]
        public void TestGetByName(MulticodecCode code)
        {
            Assert.True(MulticodecPacked.TryGet(code.Name, out var result));
            Assert.Equal(code, result);
        }

        [Theory]
        [MemberData(nameof(GetCodecs), DisableDiscoveryEnumeration = false)]
        public void TestGetByAlias(MulticodecCode code)
        {
            if (!string.IsNullOrEmpty(code.Alias))
            {
                Assert.True(MulticodecPacked.TryGet(code.Alias, out var result));
                Assert.Equal(code, result);
            }
        }

        [Theory]
        [InlineData(null, typeof(ArgumentNullException))]
        [InlineData(new byte[] { }, typeof(ArgumentNullException))]
        [InlineData(new byte[] { 255, 255 }, typeof(MulticodecException))]
        public void GivenInvalidCode_Throws(byte[] data, Type exception)
        {
            Assert.Throws(exception, () => MulticodecPacked.GetCode(data));
        }
    }
}
