namespace Core
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string s)
        {
            return string.IsNullOrEmpty(s);
        }

        public static bool HasContent(this string s)
        {
            return !string.IsNullOrEmpty(s);
        }
    }
}