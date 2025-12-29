using System;
using System.Security.Cryptography;
using System.Text;

namespace AlifPaymentSDK
{
    public static class TokenGenerator
    {
        /// <summary>
        /// Generates HMAC SHA256 token for API authentication.
        /// According to documentation, the password must be pre-hashed with the terminal key.
        /// Token generation formula: HMAC_SHA256(dataToSign, HMAC_SHA256(password, key))
        /// </summary>
        public static string GenerateToken(string dataToSign, string terminalPassword, string terminalID)
        {
            // Step 1: Pre-hash the password with terminal ID (key)
            var hashedPassword = GenerateHMAC(terminalPassword, terminalID);

            // Step 2: Generate final token using the hashed password
            var token = GenerateHMAC(dataToSign, hashedPassword);

            return token;
        }

        /// <summary>
        /// Generates HMAC-SHA256 hash and returns it as a hex string
        /// </summary>
        private static string GenerateHMAC(string data, string secret)
        {
            var encoding = new UTF8Encoding();
            var keyBytes = encoding.GetBytes(secret);
            var dataBytes = encoding.GetBytes(data);

            using (var hmac = new HMACSHA256(keyBytes))
            {
                var hashBytes = hmac.ComputeHash(dataBytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }
    }
}

