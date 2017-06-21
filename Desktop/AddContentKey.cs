using Axinom.Toolkit;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Axinom.Drm.Powershell
{
    [Cmdlet("Add", "ContentKey")]
    public sealed class AddContentKey : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public Hashtable LicenseToken { get; set; }

        [Parameter(Mandatory = true)]
        public Guid KeyId { get; set; }

        [Parameter(Mandatory = true)]
        public string KeyAsBase64 { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateLength(64, 64)]
        public string CommunicationKeyAsHex { get; set; }

        protected override void ProcessRecord()
        {
            var communicationKey = Helpers.Convert.HexStringToByteArray(CommunicationKeyAsHex);
            if (communicationKey.Length != 32)
                throw new NotSupportedException("Communication key must be 256 bits long.");

            var contentKey = Convert.FromBase64String(KeyAsBase64);
            if (contentKey.Length != 16)
                throw new NotSupportedException("Content key must be 128 bits long.");

            LicenseTokenLogic.AddKey(LicenseToken, KeyId, contentKey, communicationKey);

            WriteObject(LicenseToken);
        }

        private static List<Hashtable> EnsureKeys(Hashtable licenseToken)
        {
            var keys = licenseToken["keys"] as List<Hashtable> ?? new List<Hashtable>();
            licenseToken["keys"] = keys;
            return keys;
        }
    }
}
