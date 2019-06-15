using Axinom.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Axinom.Drm.PowerShell
{
    static class LicenseTokenLogic
    {
        public static void AddKey(Hashtable licenseToken, Guid keyId, byte[] keyValue, byte[] communicationKey, string keyUsagePolicyName)
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

            if (!string.IsNullOrWhiteSpace(keyUsagePolicyName))
                key["usage_policy"] = keyUsagePolicyName;

            var keys = OpenInlineKeysContainer(licenseToken);
            keys.Add(key);
        }

        private static List<Hashtable> OpenInlineKeysContainer(Hashtable licenseToken)
        {
            var keysSource = licenseToken["content_keys_source"] as Hashtable ?? new Hashtable();
            licenseToken["content_keys_source"] = keysSource;

            var inlineKeys = keysSource["inline"] as List<Hashtable> ?? new List<Hashtable>();
            keysSource["inline"] = inlineKeys;

            return inlineKeys;
        }
    }
}
