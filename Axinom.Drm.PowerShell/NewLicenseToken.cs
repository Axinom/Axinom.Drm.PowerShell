using System.Collections;
using System.Management.Automation;

namespace Axinom.Drm.PowerShell
{
    [Cmdlet("New", "LicenseToken")]
    public sealed class NewLicenseToken : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(new Hashtable()
            {
                { "type", "entitlement_message" },
                { "version", 2 }
            });
        }
    }
}
