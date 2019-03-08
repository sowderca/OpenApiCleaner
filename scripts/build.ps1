#!/usr/bin/env pwsh
#Requires -Version 5
#Requires -Modules Microsoft.PowerShell.Utility

using namespace System;
using namespace System.Net;
using namespace System.Security.Principal;
using namespace System.Runtime.InteropServices;

Set-StrictMode -Version 'Latest';

Import-Module 'Microsoft.PowerShell.Utility';

[bool] $dotnet = (Get-Command -CommandType Application -Name '*dotnet*' -All) -as [bool];

if ([RuntimeInformation]::IsOSPlatform([OSPlatform]::Linux)) {
    if (!($dotnet)) {
        wget -q "https://packages.microsoft.com/config/ubuntu/18.04/packages-microsoft-prod.deb"
        sudo dpkg --install packages-microsoft-prod.deb;
        sudo add-apt-repository universe;
        sudo apt install apt-transport-https;
        sudo apt update;
        sudo apt install dotnet-sdk-2.2;
    }
    dotnet build --configuration release --runtime linux-x64;
} elseif ([RuntimeInformation]::IsOSPlatform([OSPlatform]::OSX)) {
    if (!($dotnet)) {
        if (!(Get-Command -CommandType Application -Name '*brew*' -All) -as [bool]) {
            ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
        }
        brew cask install dotnet-sdk;
    }
    dotnet build --configuration release --runtime osx-x64;
} elseif ([RuntimeInformation]::IsOSPlatform([OSPlatform]::Windows)) {
    [WindowsPrincipal] $principal = [WindowsPrincipal]::new([WindowsIdentity]::GetCurrent());
    if (!($principal.IsInRole([WindowsBuiltInRole]::Administrator))) {
        if ((([WMISearcher] 'select BuildNumber from Win32_OperatingSystem').Get().BuildNumber -ge 6000) -as [bool]) {
            Start-Process -FilePath 'powershell.exe' -Verb 'RunAs' -ArgumentList @('-File', $MyInvocation.MyCommand.Path, $MyInvocation.UnboundArguments);
            exit $LASTEXITCODE;
        }
    }
    if (!($dotnet)) {
        if (!($env:chocolateyInstall -as [bool])) {
            Invoke-Expression "$(curl.exe "https://chocolatey.org/install.ps1" | Out-String)";
            &RefreshEnv.cmd;
        }
        choco install dotnetcore-sdk -y;
    }
    dotnet build --configuration release --runtime win-x64;
}