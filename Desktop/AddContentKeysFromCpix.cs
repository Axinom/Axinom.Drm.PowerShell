using Axinom.Cpix;
using Axinom.Toolkit;
using System;
using System.Collections;
using System.IO;
using System.Management.Automation;

namespace Axinom.Drm.Powershell
{
    [Cmdlet("Add", "ContentKeysFromCpix")]
    public sealed class AddContentKeysFromCpix : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public Hashtable LicenseToken { get; set; }

        [Parameter(Mandatory = true)]
        public string Path{ get; set; }

        [Parameter(Mandatory = true)]
        [ValidateLength(64, 64)]
        public string CommunicationKeyAsHex { get; set; }

        protected override void ProcessRecord()
        {
            if (!File.Exists(Path))
                throw new PSArgumentException("CPIX file not found: " + Path, "Path");

            var cpix = CpixDocument.Load(Path);
            if (!cpix.ContentKeysAreReadable)
                throw new NotSupportedException("The content keys in the CPIX file are encrypted. This PowerShell command does not currently support decryption of encryted content keys.");

            var communicationKey = Helpers.Convert.HexStringToByteArray(CommunicationKeyAsHex);
            if (communicationKey.Length != 32)
                throw new NotSupportedException("Communication key must be 256 bits long.");

            foreach (var key in cpix.ContentKeys)
            {
                WriteVerbose("Adding key: " + key.Id);

                LicenseTokenLogic.AddKey(LicenseToken, key.Id, key.Value, communicationKey);
            }

            WriteObject(LicenseToken);
        }
    }
}
