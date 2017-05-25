using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Multiformats.Codec.Codecs;

namespace Multiformats.Codec.CLI
{
    class Program
    {
        private static bool mcwrap = false;
        private static bool msgio = false;
        private static bool verbose = true;
        private static bool @async = false;

        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                PrintUsage();
                return;
            }

            var i = 0;
            var command = args[i++];
            while (i < args.Length && args[i].StartsWith("--"))
            {
                switch (args[i].TrimStart('-').ToLower())
                {
                    case "mcwrap":
                        mcwrap = true;
                        break;
                    case "msgio":
                        msgio = true;
                        break;
                    case "verbose":
                        verbose = true;
                        break;
                    case "async":
                        @async = true;
                        break;
                }
                i++;
            }

            Console.WriteLine($"Options: wrap={mcwrap}, msgio={msgio}, verbose={verbose}, async={async}");

            using (var w = new StreamWriter(Console.OpenStandardOutput(), Encoding.ASCII))
            {
                using (var mem = new MemoryStream())
                {
                    Console.OpenStandardInput().CopyTo(mem);
                    mem.Seek(0, SeekOrigin.Begin);

                    using (var r = new StreamReader(mem))
                    {

                        switch (command.ToLower())
                        {
                            case "header":
                                Header(w, args[i]);
                                break;
                            case "headers":
                                Headers(w, r);
                                break;
                            case "paths":
                                Paths(w, r);
                                break;
                            case "wrap":
                                Wrap(w, r, args[i]);
                                break;
                            case "filter":
                                Filter(w, r, args[i]);
                                break;
                            case "recode":
                                if (@async)
                                    RecodeAsync(w, r, args[i]).Wait();
                                else
                                    Recode(w, r, args[i]);
                                break;
                            case "h2p":
                                H2P(w, r);
                                break;
                            case "p2h":
                                P2H(w, r);
                                break;
                            default:
                                PrintUsage();
                                break;
                        }
                    }
                }
            }

            if (Debugger.IsAttached)
                Console.ReadLine();
        }

        private static void Log(string message)
        {
            if (verbose)
                Console.WriteLine(message);
        }

        private static MuxCodec Codec()
        {
            var m = MuxCodec.Standard;
            m.Wrap = mcwrap;
            return m;
        }

        private static void Decode(StreamReader r, Func<MuxCodec, dynamic, bool> next)
        {
            var c = Codec();
            var dec = c.Decoder(r.BaseStream);

            try
            {
                while (true)
                {
                    var v = dec.Decode<dynamic>();

                    if (!next(c, v))
                        break;
                }
            }
            catch (EndOfStreamException e)
            {
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error: {e.Message}");
            }
        }

        private static async Task DecodeAsync(StreamReader r, Func<MuxCodec, dynamic, Task<bool>> next)
        {
            var c = Codec();
            var dec = c.Decoder(r.BaseStream);

            try
            {
                while (true)
                {
                    var v = await dec.DecodeAsync<dynamic>(CancellationToken.None);

                    if (!await next(c, v))
                        break;
                }
            }
            catch (EndOfStreamException e)
            {
            }
            catch (Exception e)
            {
                Console.Error.WriteLine($"Error: {e.Message}");
            }
        }

        private static ICodec CodecWithPath(string path)
        {
            var hdr = Multicodec.Header(Encoding.UTF8.GetBytes(path));

            return MuxCodec.CodecWithHeader(hdr, MuxCodec.Standard.Codecs);
        }

        private static void Recode(StreamWriter w, StreamReader r, string path)
        {
            var codec = CodecWithPath(path);
            if (codec == null)
                throw new Exception($"unknown codec {path}");

            var enc = codec.Encoder(w.BaseStream);

            Decode(r, (c, v) =>
            {
                enc.Encode(v);
                return true;
            });
        }

        private static async Task RecodeAsync(StreamWriter w, StreamReader r, string path)
        {
            var codec = CodecWithPath(path);
            if (codec == null)
                throw new Exception($"unknown codec {path}");

            var enc = codec.Encoder(w.BaseStream);

            await DecodeAsync(r, async (c, v) =>
            {
                await enc.EncodeAsync(v, CancellationToken.None);
                return true;
            });
        }

        private static void Filter(StreamWriter w, StreamReader r, string path)
        {
            throw new NotImplementedException();
            var hdr = Multicodec.Header(Encoding.UTF8.GetBytes(path));

            Decode(r, (codec, value) =>
            {
                if (!codec.Last.Header.SequenceEqual(hdr))
                    return true;

                return true;
            });
        }

        private static void Wrap(StreamWriter w, StreamReader r, string path)
        {
            var mcc = CodecWithPath(path);
            if (mcc == null)
                throw new Exception($"unknown codec {path}");

            var hdrs = Encoding.UTF8.GetString(mcc.Header);

            Action<ICodec, ICodec> wrapRT = (c, mc) =>
            {
                var v = c.Decoder(r.BaseStream).Decode<dynamic>();
                mc.Encoder(w.BaseStream).Encode(v);
            };

            if (hdrs == JsonCodec.HeaderMsgioPath)
                wrapRT(JsonCodec.CreateCodec(true), mcc);
            else if (hdrs == JsonCodec.HeaderPath)
                wrapRT(JsonCodec.CreateCodec(false), mcc);
            else if (hdrs == CborCodec.HeaderPath)
                wrapRT(CborCodec.CreateCodec(), mcc);
            else
                throw new Exception($"wrap unsupported for codec {hdrs}");
        }

        private static void Paths(StreamWriter w, StreamReader r)
        {
            Decode(r, (codec, obj) =>
            {
                var p = Multicodec.HeaderPath(codec.Last.Header);
                w.WriteLine(Encoding.UTF8.GetString(p));
                return true;
            });
        }

        private static void Headers(StreamWriter w, StreamReader r)
        {
            Decode(r, (codec, obj) =>
            {
                w.Write(Encoding.UTF8.GetString(codec.Last.Header));
                return true;
            });
        }

        private static void Header(StreamWriter w, string path)
        {
            w.Write(Multicodec.Header(Encoding.UTF8.GetBytes(path)));
        }

        private static void P2H(StreamWriter w, StreamReader r)
        {
            while (true)
            {
                var p = r.ReadLine();
                if (string.IsNullOrEmpty(p))
                    break;

                var hdr = Multicodec.Header(Encoding.UTF8.GetBytes(p));
                w.Write(hdr);
            }
        }

        private static void H2P(StreamWriter w, StreamReader r)
        {
            while (true)
            {
                var hdr = Multicodec.ReadHeader(r.BaseStream);
                if (hdr == null || hdr.Length == 0)
                    return;

                var p = Encoding.UTF8.GetString(Multicodec.HeaderPath(hdr));
                w.WriteLine(p);
            }
        }

        private static void PrintUsage()
        {
            Console.WriteLine(string.Join(Environment.NewLine,
                "multicodec - tool to inspect and manipulate mixed codec streams",
                "",
                "Usage",
                "\tcat rawjson | multicodec wrap /json/msgio >mcjson"));
        }
    }
}
