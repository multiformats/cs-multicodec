using System;

namespace Multiformats.Codec
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class StringValueAttribute : Attribute
    {
        public string Value { get; }

        public StringValueAttribute(string value)
            : base()
        {
            Value = value;
        }
    }
}