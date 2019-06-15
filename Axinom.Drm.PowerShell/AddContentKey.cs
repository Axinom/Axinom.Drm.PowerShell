using System;
using System.Collections;
using System.Management.Automation;

namespace Axinom.Drm.PowerShell
{
    [Cmdlet("Add", "ContentKey")]
    public sealed class AddContentKey : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public Hashtable LicenseToken { get; set; }

        [Parameter(Mandatory = true)]
        public Guid KeyId { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateLength(24, 24)]
        public string KeyAsBase64 { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateLength(44, 44)]
        public string CommunicationKeyAsBase64 { get; set; }

        [Parameter()]
        public string KeyUsagePolicyName { get; set; }

        protected override void ProcessRecord()
        {
            var communicationKey = Convert.FromBase64String(CommunicationKeyAsBase64);
            if (communicationKey.Length != 32)
                throw new NotSupportedException("Communication key must be 256 bits long.");

            var contentKey = Convert.FromBase64String(KeyAsBase64);
            if (contentKey.Length != 16)
                throw new NotSupportedException("Content key must be 128 bits long.");

            LicenseTokenLogic.AddKey(LicenseToken, KeyId, contentKey, communicationKey, KeyUsagePolicyName);

            WriteObject(LicenseToken);
        }
    }
}
