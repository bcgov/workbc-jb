namespace WorkBC.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string Capitalize(this string s)
        {
            if (s == null)
            {
                return null;
            }

            if (s.Length >= 2)
            {
                return $"{char.ToUpper(s[0])}{s.Substring(1).ToLower()}";
            }

            if (s.Length == 1)
            {
                return s.ToUpper();
            }

            return string.Empty;
        }

        public static string Truncate(this string value, int maxLength)
        {
            if (!string.IsNullOrEmpty(value) && value.Length > maxLength)
            {
                return value.Substring(0, maxLength);
            }

            return value;
        }
    }
}