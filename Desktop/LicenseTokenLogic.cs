using Axinom.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Axinom.Drm.Powershell
{
    static class LicenseTokenLogic
    {
        public static void AddKey(Hashtable licenseToken, Guid keyId, byte[] keyValue, byte[] communicationKey)
        {
            byte[] encryptedKey;

            using (var aes = Aes.Create())
            {
                aes.KeySize = 256;
                aes.Padding = PaddingMode.None;
                aes.Mode = CipherMode.CBC;

                aes.IV = keyId.ToBigEndianByteArray();
                aes.Key = communicationKey;

                using (var encryptor = aes.CreateEncryptor())
                    encryptedKey = encryptor.TransformFinalBlock(keyValue, 0, keyValue.Length);
            }

            var key = new Hashtable
            {
                { "id", keyId.ToString() },
                { "encrypted_key", Convert.ToBase64String(encryptedKey) }
            };

            var keys = EnsureKeys(licenseToken);
            keys.Add(key);
        }

        private static List<Hashtable> EnsureKeys(Hashtable licenseToken)
        {
            var keys = licenseToken["keys"] as List<Hashtable> ?? new List<Hashtable>();
            licenseToken["keys"] = keys;
            return keys;
        }
    }
}
