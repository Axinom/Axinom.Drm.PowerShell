Axinom DRM PowerShell module
===================

This is a PowerShell module that simplifies working with Axinom DRM.

Platform compatibility
======================

This library is compatible with all platforms supported by PowerShell and PowerShell Core.

Features
========

The following features are implemented:

* Commands for easily creating Axinom DRM license tokens (token version 2).

Installation
============

`Install-Module Axinom.Drm`

Quick start: creating a license token
=========================

```powershell
# Communication keys are only provided to Axinom DRM customers.
# Evaluation access can be provided - contact Axinom for details.
$communicationKeyAsBase64 = "TODO: put base64 form of communication key here"
$communicationKeyId = "TODO: put communication key ID here"

$token = New-LicenseToken

# You can customize any part of the token. Its structure is described by the Axinom DRM documentation.
# As an example, we specify the license validity start and end timestamps here.
$token.license = @{
    start_datetime = (Get-Date).AddMinutes(-5).ToString("o")
    expiration_datetime = (Get-Date).AddHours(1).ToString("o")
}

# You can can define policies that apply to the usage of keys by client devices.
$permissivePolicy = @{
    name = "Permissive"

    playready = @{
        # Here is an example for how to make the PlayReady DRM configuration maximally permissive.
        # This lets you play content on virtual machines and pre-production devices, for easy testing.
        min_device_security_level = 150
        play_enablers = @(
            "786627D8-C2A6-44BE-8F88-08AE255B01A7"
        )
    }
}

$token.content_key_usage_policies = @(
    $permissivePolicy
)

# Add a manually specified content key. Use the defined permissive policy.
$token = $token | Add-ContentKey -KeyId "8e413433-1e91-47d4-b548-5abbf4f6564e" -KeyAsBase64 "WMDlg3QKs72fEKsquqnPFg==" -CommunicationKeyAsBase64 $communicationKeyAsBase64 -KeyUsagePolicyName $permissivePolicy.name

# Add a set of content keys from a CPIX document. Use the defined permissive policy.
$token = $token | Add-ContentKeysFromCpix -Path "C:\tmp\threekey.xml" -CommunicationKeyAsBase64 $communicationKeyAsBase64  -KeyUsagePolicyName $permissivePolicy.name

# Generate the final signed form of the license token, suitable for use in a license request.
# The -Verbose flag will also output the license token in its raw form, before signing.
$token | Export-LicenseToken -CommunicationKeyId $communicationKeyId -CommunicationKeyAsBase64 $communicationKeyAsBase64 -Verbose
```