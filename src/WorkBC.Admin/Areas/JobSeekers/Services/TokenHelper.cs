using System;
using System.Security.Cryptography;

namespace WorkBC.Admin.Areas.JobSeekers.Services
{
    /// <summary>
    ///     Uses RNGCryptoServiceProvider to generate a 200 character token
    /// </summary>
    public static class TokenHelper
    {
        /// <summary>
        ///     https://stackoverflow.com/q/39852975/2616170
        /// </summary>
        public static string GenerateRandomStringToken()
        {
            byte[] randomBytes = GenerateRandomBytes(1200);
            string result = UrlTokenEncode(randomBytes);
            if (result.Length > 200)
            {
                result = result.Substring(0, 200);
            }

            return result;
        }

        private static byte[] GenerateRandomBytes(int length)
        {
            var bytes = new byte[length];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return bytes;
        }

        /// <summary>
        ///     https://stackoverflow.com/a/57645538/2616170
        /// </summary>
        private static string UrlTokenEncode(byte[] input)
        {
            // Step 1: Do a Base64 encoding
            string base64Str = Convert.ToBase64String(input);

            int endPos;

            // Step 2: Find how many padding chars are present in the end
            for (endPos = base64Str.Length; endPos > 0; endPos--)
            {
                if (base64Str[endPos - 1] != '=') // Found a non-padding char!
                {
                    break; // Stop here
                }
            }

            // Step 3: Create char array to store all non-padding chars,
            //      plus a char to indicate how many padding chars are needed
            var base64Chars = new char[endPos + 1];

            // Store a char at the end, to indicate how many padding chars are needed
            base64Chars[endPos] = (char) ('0' + base64Str.Length - endPos); 

            // Step 3: Copy in the other chars. Transform the "+" to "-", and "/" to "_"
            for (var iter = 0; iter < endPos; iter++)
            {
                char c = base64Str[iter];

                switch (c)
                {
                    case '+':
                        base64Chars[iter] = '-';
                        break;

                    case '/':
                        base64Chars[iter] = '_';
                        break;

                    case '=':
                        base64Chars[iter] = c;
                        break;

                    default:
                        base64Chars[iter] = c;
                        break;
                }
            }

            return new string(base64Chars);
        }
    }
}