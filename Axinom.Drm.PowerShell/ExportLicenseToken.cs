using Jose;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Management.Automation;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Axinom.Drm.PowerShell
{
    [Cmdlet("Export", "LicenseToken")]
    public sealed class ExportLicenseToken : PSCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true)]
        public Hashtable LicenseToken { get; set; }

        [Parameter(Mandatory = true)]
        public Guid CommunicationKeyId { get; set; }

        [Parameter(Mandatory = true)]
        [ValidateLength(44, 44)]
        public string CommunicationKeyAsBase64 { get; set; }

        protected override void ProcessRecord()
        {
            var communicationKey = Convert.FromBase64String(CommunicationKeyAsBase64);
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
            var json = JsonConvert.SerializeObject(envelope);
            WriteVerbose(json);

            // The output is the signed JOSE object, short-form serialized.
            var joseObject = JWT.Encode(json, communicationKey, JwsAlgorithm.HS256);

            WriteObject(joseObject);
        }

        static ExportLicenseToken()
        {
            if (!RuntimeInformation.FrameworkDescription.Contains(".NET Framework"))
                return;

            // Assembly binding redirects on PowerShell Desktop are broken. We need to do it manually.
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
        }

        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = new AssemblyName(args.Name);

            if (name.Name == "Newtonsoft.Json")
                return typeof(JsonSerializer).Assembly;

            return null;
        }
    }
}
