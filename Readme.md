Axinom DRM PowerShell module
===================

This is a PowerShell 5 module that simplifies working with Axinom DRM.

Platform compatibility
======================

This library is compatible with .NET Framework 4.6.2 and newer on Windows 10 or Windows Server 2016.

Features
========

The following features are implemented:

* Commands for easily creating license tokens

Installation
============

Download from Releases. A more convenient mechanism may be provided in the future.

Quick start: creating a license token
=========================

```powershell
# TODO: Update this to point to the location you extracted the release package.
Import-Module c:\AxinomDRM.PowerShell\Axinom.Drm.Powershell.Desktop.dll

$communicationKey = "TODO: put hex form of communication key here"
$communicationKeyId = "TODO: put communication key ID here"

$token = New-LicenseToken

# You can customize any part of the token. Its structure is described by the Axinom DRM documentation.
# As an example, we specify the validity start and end timestamps here.
$token.begin_date = (Get-Date).AddMinutes(-5).ToString("o")
$token.expiration_date = (Get-Date).AddHours(1).ToString("o")

# Add a manually specified content key.
$token = $token | Add-ContentKey -KeyId "8e413433-1e91-47d4-b548-5abbf4f6564e" -KeyAsBase64 "WMDlg3QKs72fEKsquqnPFg==" -CommunicationKeyAsHex $communicationKey

# Add a set of content keys from a CPIX document.
$token = $token | Add-ContentKeysFromCpix -Path "C:\tmp\threekey.xml" -CommunicationKeyAsHex $communicationKey

# Generate the final signed form of the license token, suitable for use in a license request.
# The -Verbose flag will also output the license token in its raw form, before signing.
$token | Export-LicenseToken -CommunicationKeyId $communicationKeyId -CommunicationKeyAsHex $communicationKey -Verbose
```