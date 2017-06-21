using System.Collections;
using System.Collections.Generic;
using System.Management.Automation;

namespace Axinom.Drm.Powershell
{
    [Cmdlet("New", "LicenseToken")]
    public sealed class NewLicenseToken : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(new Hashtable()
            {
                { "type", "entitlement_message" },
                { "keys", new List<Hashtable>() }
            });
        }
    }
}
