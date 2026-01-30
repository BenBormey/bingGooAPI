using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace bingGooAPI.Helpers
{
    public static class AesEncryptionHelper
    {
        private const int KeySize = 256;
        private const int Iterations = 100_000;

        public static string Encrypt(string plainText, string password)
        {
            byte[] salt = RandomNumberGenerator.GetBytes(16);
            byte[] iv = RandomNumberGenerator.GetBytes(16);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] key = pbkdf2.GetBytes(KeySize / 8);

            using var aes = Aes.Create();
            aes.KeySize = KeySize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            using var encryptor = aes.CreateEncryptor();
            using var ms = new MemoryStream();
            using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
            {
                byte[] data = Encoding.UTF8.GetBytes(plainText);
                cs.Write(data, 0, data.Length);
                cs.FlushFinalBlock();
            }

            byte[] cipherText = ms.ToArray();

            // Combine: salt + iv + cipher
            byte[] result = new byte[salt.Length + iv.Length + cipherText.Length];
            Buffer.BlockCopy(salt, 0, result, 0, salt.Length);
            Buffer.BlockCopy(iv, 0, result, salt.Length, iv.Length);
            Buffer.BlockCopy(cipherText, 0, result, salt.Length + iv.Length, cipherText.Length);

            return Convert.ToBase64String(result);
        }

        public static string Decrypt(string encryptedText, string password)
        {
            byte[] fullData = Convert.FromBase64String(encryptedText);

            byte[] salt = new byte[16];
            byte[] iv = new byte[16];
            byte[] cipherText = new byte[fullData.Length - 32];

            Buffer.BlockCopy(fullData, 0, salt, 0, 16);
            Buffer.BlockCopy(fullData, 16, iv, 0, 16);
            Buffer.BlockCopy(fullData, 32, cipherText, 0, cipherText.Length);

            using var pbkdf2 = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
            byte[] key = pbkdf2.GetBytes(KeySize / 8);

            using var aes = Aes.Create();
            aes.KeySize = KeySize;
            aes.Mode = CipherMode.CBC;
            aes.Padding = PaddingMode.PKCS7;
            aes.Key = key;
            aes.IV = iv;

            using var decryptor = aes.CreateDecryptor();
            using var ms = new MemoryStream(cipherText);
            using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
            using var reader = new StreamReader(cs, Encoding.UTF8);

            return reader.ReadToEnd();
        }
    }
}
