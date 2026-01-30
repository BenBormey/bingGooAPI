
    using Microsoft.AspNetCore.Cryptography.KeyDerivation;
    using System;
    using System.Runtime.CompilerServices;
    using System.Security.Cryptography;

        namespace bingGooAPI.Models
        {
    public static class PasswordHasher
        {
            private const KeyDerivationPrf Prf = KeyDerivationPrf.HMACSHA256;
            private const int IterationCount = 100_000;   // modern standard
            private const int SaltSize = 128 / 8;         // 16 bytes
            private const int KeySize = 256 / 8;          // 32 bytes

            /// <summary>
            /// Hash password using PBKDF2 (ASP.NET Identity compatible format)
            /// </summary>
            public static string HashPassword(string password)
            {
                if (string.IsNullOrWhiteSpace(password))
                    throw new ArgumentException("Password cannot be empty.");

                byte[] salt = RandomNumberGenerator.GetBytes(SaltSize);

                byte[] subkey = KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: Prf,
                    iterationCount: IterationCount,
                    numBytesRequested: KeySize);

                // Format:
                // [0]     = format marker (0x03)
                // [1..4]  = PRF
                // [5..8]  = iteration count
                // [9..12] = salt length
                // [13..]  = salt + subkey
                var outputBytes = new byte[13 + salt.Length + subkey.Length];

                outputBytes[0] = 0x03;
                WriteNetworkByteOrder(outputBytes, 1, (uint)Prf);
                WriteNetworkByteOrder(outputBytes, 5, (uint)IterationCount);
                WriteNetworkByteOrder(outputBytes, 9, (uint)SaltSize);

                Buffer.BlockCopy(salt, 0, outputBytes, 13, salt.Length);
                Buffer.BlockCopy(subkey, 0, outputBytes, 13 + salt.Length, subkey.Length);

                return Convert.ToBase64String(outputBytes);
            }

            /// <summary>
            /// Verify password against stored hash
            /// </summary>
            public static bool VerifyPassword(string hashedPassword, string password)
            {
                if (string.IsNullOrWhiteSpace(hashedPassword) || string.IsNullOrWhiteSpace(password))
                    return false;

                byte[] decodedHash = Convert.FromBase64String(hashedPassword);

                try
                {
                    var prf = (KeyDerivationPrf)ReadNetworkByteOrder(decodedHash, 1);
                    int iterCount = (int)ReadNetworkByteOrder(decodedHash, 5);
                    int saltLength = (int)ReadNetworkByteOrder(decodedHash, 9);

                    if (saltLength < SaltSize)
                        return false;

                    byte[] salt = new byte[saltLength];
                    Buffer.BlockCopy(decodedHash, 13, salt, 0, salt.Length);

                    int subkeyLength = decodedHash.Length - 13 - salt.Length;
                    if (subkeyLength < KeySize)
                        return false;

                    byte[] expectedSubkey = new byte[subkeyLength];
                    Buffer.BlockCopy(decodedHash, 13 + salt.Length, expectedSubkey, 0, expectedSubkey.Length);

                    byte[] actualSubkey = KeyDerivation.Pbkdf2(
                        password: password,
                        salt: salt,
                        prf: prf,
                        iterationCount: iterCount,
                        numBytesRequested: subkeyLength);

                    return FixedTimeEquals(actualSubkey, expectedSubkey);
                }
                catch
                {
                    return false;
                }
            }

            private static uint ReadNetworkByteOrder(byte[] buffer, int offset)
            {
                return ((uint)buffer[offset] << 24)
                     | ((uint)buffer[offset + 1] << 16)
                     | ((uint)buffer[offset + 2] << 8)
                     | buffer[offset + 3];
            }

            private static void WriteNetworkByteOrder(byte[] buffer, int offset, uint value)
            {
                buffer[offset] = (byte)(value >> 24);
                buffer[offset + 1] = (byte)(value >> 16);
                buffer[offset + 2] = (byte)(value >> 8);
                buffer[offset + 3] = (byte)value;
            }

            // Constant-time comparison (anti timing attack)
            [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
            private static bool FixedTimeEquals(byte[] a, byte[] b)
            {
                if (a.Length != b.Length)
                    return false;

                int diff = 0;
                for (int i = 0; i < a.Length; i++)
                {
                    diff |= a[i] ^ b[i];
                }
                return diff == 0;
            }
        }
    }
