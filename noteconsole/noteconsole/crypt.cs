using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace noteconsole
{
    internal class crypt
    {
        public static async Task<byte[]> Encrypt(byte[] dataToEncrypt, string? key)
        {
            if (dataToEncrypt == null || key == null)
            {
                return null;
            }

            var instance = new crypt();
            return await instance.EncryptAsync(dataToEncrypt, key);
        }
        public static async Task<byte[]> Decrypt(byte[] dataToDecrypt, string? key)
        {
            if (dataToDecrypt == null || key == null)
            {
                return null;
            }

            var instance = new crypt();
            return await instance.DecryptAsync(dataToDecrypt, key);
        }

        private byte[] DeriveKeyFromPassword(string? password)
        {
            var emptySalt = Array.Empty<byte>();
            var iterations = 1000;
            var desiredKeyLength = 16;
            var hashMethod = HashAlgorithmName.SHA384;
            return Rfc2898DeriveBytes.Pbkdf2(Encoding.Unicode.GetBytes(password),
                                             emptySalt,
                                             iterations,
                                             hashMethod,
                                             desiredKeyLength);
        }

        private byte[] IV =
        {
        0x01, 0x02, 0x03, 0x04, 0x05, 0x06, 0x07, 0x08,
        0x09, 0x10, 0x11, 0x12, 0x13, 0x14, 0x15, 0x16
        };
        public async Task<byte[]> EncryptAsync(byte[] clearData, string? passphrase)
        {
            using Aes aes = Aes.Create();
            aes.Key = DeriveKeyFromPassword(passphrase);
            aes.IV = IV;

            using MemoryStream output = new();
            using CryptoStream cryptoStream = new(output, aes.CreateEncryptor(), CryptoStreamMode.Write);

            await cryptoStream.WriteAsync(clearData);
            await cryptoStream.FlushFinalBlockAsync();

            return output.ToArray();
        }

        // DecryptAsync now returns byte[] instead of string
        public async Task<byte[]> DecryptAsync(byte[] encrypted, string? passphrase)
        {
            using Aes aes = Aes.Create();
            aes.Key = DeriveKeyFromPassword(passphrase);
            aes.IV = IV;

            using MemoryStream input = new(encrypted);
            using CryptoStream cryptoStream = new(input, aes.CreateDecryptor(), CryptoStreamMode.Read);

            using MemoryStream output = new();
            await cryptoStream.CopyToAsync(output);

            return output.ToArray();
        }
    }
}
