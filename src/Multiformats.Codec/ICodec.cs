using System.IO;

namespace Multiformats.Codec
{
    public interface ICodec
    {
        byte[] Header { get; }
        //string HeaderPath { get; }

        ICodecEncoder Encoder(Stream stream);
        ICodecDecoder Decoder(Stream stream);
    }
}