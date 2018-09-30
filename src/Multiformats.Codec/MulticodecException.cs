using System;

namespace Multiformats.Codec
{
    public class MulticodecException : Exception
    {
        public MulticodecException(string message)
            : base(message)
        {
        }
    }
}
