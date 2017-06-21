using Axinom.Toolkit;
using Jose;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;
using System.Web.Script.Serialization;

namespace Axinom.Drm.Powershell
{
    [Cmdlet("Export", "LicenseToken")]
    public sealed class ExportLicenseToken : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public Hashtable LicenseToken { get; set; }

        [Parameter(Mandatory = true)]
        public Guid CommunicationKeyId { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateLength(64, 64)]
        public string CommunicationKeyAsHex { get; set; }

        protected override void ProcessRecord()
        {
            var communicationKey = Helpers.Convert.HexStringToByteArray(CommunicationKeyAsHex);
            if (communicationKey.Length != 32)
                throw new NotSupportedException("Communication key must be 256 bits long.");

            var envelope = new Hashtable
            {
                { "version", 1 },
                { "com_key_id", CommunicationKeyId.ToString() },
                { "message", LicenseToken }
            };

            // Copy the expiration from the "message" structure if they are defined.
            if (LicenseToken.ContainsKey("begin_date"))
                envelope["begin_date"] = LicenseToken["begin_date"];

            if (LicenseToken.ContainsKey("expiration_date"))
                envelope["expiration_date"] = LicenseToken["expiration_date"];

            // Now we have the outer structure ready. Convert to JSON and then sign.
            // Use the JavaScriptSerializer because it can deal with Hashtables.
            var serializer = new JavaScriptSerializer();
            var json = serializer.Serialize(envelope);
            WriteVerbose(json);

            // The output is the signed JOSE object, short-form serialized.
            var joseObject = JWT.Encode(json, communicationKey, JwsAlgorithm.HS256);

            WriteObject(joseObject);
        }
    }
}
