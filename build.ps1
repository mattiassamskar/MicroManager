[CmdletBinding()]
param (
  [ValidateSet("Debug", "Release")]
  [string[]] $Configuration = "Debug"
)

& "C:\Program Files (x86)\MSBuild\14.0\Bin\MSBuild.exe" /p:Configuration=$Configuration
