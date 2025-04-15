namespace Shared.Extensions
{
    public static class StringExtensions
    {
        public static string ToLowerFirstChar(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return str;

            var chars = str.ToCharArray();
            chars[0] = char.ToLowerInvariant(chars[0]);

            return new string(chars);
        }
    }
}
