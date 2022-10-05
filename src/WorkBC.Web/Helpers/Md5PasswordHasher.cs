using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace WorkBC.Web.Helpers
{
    /// <summary>
    ///     based on https://andrewlock.net/safely-migrating-passwords-in-asp-net-core-identity-with-a-custom-passwordhasher/
    /// </summary>
    public class Md5PasswordHasher<TUser> : PasswordHasher<TUser> where TUser : class
    {
        public const byte Md5FormatByte = 0xF0;

        public override PasswordVerificationResult VerifyHashedPassword(TUser user, string hashedPassword,
            string providedPassword)
        {
            if (hashedPassword == null)
            {
                throw new ArgumentNullException(nameof(hashedPassword));
            }

            if (providedPassword == null)
            {
                throw new ArgumentNullException(nameof(providedPassword));
            }

            byte[] decodedHashedPassword = Convert.FromBase64String(hashedPassword);

            // read the format marker from the hashed password
            if (decodedHashedPassword.Length == 0)
            {
                return PasswordVerificationResult.Failed;
            }

            // ASP.NET Core uses 0x00 and 0x01, so we start at the other end
            if (decodedHashedPassword[0] == Md5FormatByte)
            {
                // replace the 0xF0 prefix in the stored password with 0x01 (ASP.NET Core Identity V3) and convert back
                decodedHashedPassword[0] = 0x01;
                string storedPassword = Convert.ToBase64String(decodedHashedPassword);

                // md5 hash the provided password
                // todo: why is md5ProvidedPassword different than what was in the old DB?
                string md5ProvidedPassword = GetMd5Hash(providedPassword);

                // call the base implementation with the new values
                PasswordVerificationResult
                    result = base.VerifyHashedPassword(user, storedPassword, md5ProvidedPassword);

                return result == PasswordVerificationResult.Success
                    ? PasswordVerificationResult.SuccessRehashNeeded
                    : result;
            }

            return base.VerifyHashedPassword(user, hashedPassword, providedPassword);
        }

        public static string GetMd5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] bytes = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                var builder = new StringBuilder(bytes.Length * 2);
                foreach (byte t in bytes)
                {
                    builder.Append(t.ToString("x2"));
                }

                return builder.ToString();

            }
        }
    }
}