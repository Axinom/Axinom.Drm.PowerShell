using Axinom.Cpix;
using System;
using System.Collections;
using System.IO;
using System.Management.Automation;

namespace Axinom.Drm.PowerShell
{
    [Cmdlet("Add", "ContentKeysFromCpix")]
    public sealed class AddContentKeysFromCpix : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public Hashtable LicenseToken { get; set; }

        [Parameter(Mandatory = true)]
        public string Path{ get; set; }

        [Parameter(Mandatory = true)]
        [ValidateLength(44, 44)]
        public string CommunicationKeyAsBase64 { get; set; }

        [Parameter()]
        public string KeyUsagePolicyName { get; set; }

        protected override void ProcessRecord()
        {
            if (!File.Exists(Path))
                throw new PSArgumentException("CPIX file not found: " + Path, "Path");

            var cpix = CpixDocument.Load(Path);
            if (!cpix.ContentKeysAreReadable)
                throw new NotSupportedException("The content keys in the CPIX file are encrypted. This PowerShell command does not currently support decryption of encryted content keys.");

            var communicationKey = Convert.FromBase64String(CommunicationKeyAsBase64);
            if (communicationKey.Length != 32)
                throw new NotSupportedException("Communication key must be 256 bits long.");

            foreach (var key in cpix.ContentKeys)
            {
                WriteVerbose("Adding key: " + key.Id);

                LicenseTokenLogic.AddKey(LicenseToken, key.Id, key.Value, communicationKey, KeyUsagePolicyName);
            }

            WriteObject(LicenseToken);
        }
    }
}
