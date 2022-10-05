using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace WorkBC.Shared.Utilities
{
    public static class HashingHelper
    {            // Helper Functions
        public static string Hash2Strings(string string1, string string2)
        {
            return GetHashString((string1 ?? "") + (string2 ?? ""));
        }

        // NOTE: This is not used for security purposes and hashing does not need to be perfect.
        // SHA1 was selected because it is very fast.  
        private static string GetHashString(string inputString)
        {
            var sb = new StringBuilder();
            byte[] bytes = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(inputString));

            foreach (byte b in bytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
    }
}
