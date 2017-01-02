using System.Reflection;

namespace Multiformats.Codec
{
    public static class MulticodecPackedExtensions
    {
        public static string GetString(this MulticodecCode code)
        {
            var memberInfo = code.GetType().GetMember(code.ToString());
            if (memberInfo != null && memberInfo.Length > 0)
            {
                var attr = memberInfo[0].GetCustomAttribute<StringValueAttribute>();
                if (attr != null)
                    return attr.Value;
            }

            return code.ToString();
        }
    }
}