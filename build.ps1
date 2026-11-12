<#
.SYNOPSIS
    Builds Bubble Buffs Plus and packages it as dist\BubbleBuffsPlus-<version>.zip.

.PARAMETER Install
    Also copies the built mod into the game's Mods folder (the game must be closed).
    Buff setups already saved there (UserSettings\bubblebuff-*.json) are kept.

.EXAMPLE
    .\build.ps1
    .\build.ps1 -Install
#>
[CmdletBinding()]
param(
    [switch]$Install
)

$ErrorActionPreference = 'Stop'
$root = $PSScriptRoot
$modId = 'BubbleBuffsPlus'
$projectDir = Join-Path $root 'BubbleBuffs'

# The game folder: GamePath.props if the project wrote or was given one, otherwise the default Steam path.
$gamePath = 'C:\Program Files (x86)\Steam\steamapps\common\Pathfinder Second Adventure'
$props = Join-Path $root 'GamePath.props'
if (Test-Path $props) {
    $configured = ([xml](Get-Content $props -Raw)).Project.PropertyGroup.WrathInstallDir
    if ($configured) { $gamePath = $configured }
}
if (-not (Test-Path (Join-Path $gamePath 'Wrath_Data\Managed\Assembly-CSharp.dll'))) {
    throw "Pathfinder: Wrath of the Righteous not found at '$gamePath'."
}

# Info.json is the single source of the version shown by Unity Mod Manager.
$info = Get-Content (Join-Path $projectDir 'Info.json') -Raw | ConvertFrom-Json
$version = $info.Version

dotnet build (Join-Path $projectDir 'BubbleBuffs.csproj') -c Release --nologo -v q `
    "-p:Version=$version" "-p:WrathInstallDir=$gamePath" "-p:SolutionDir=$root\"
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

# Everything the build put in its output folder belongs to the mod, except debug symbols
# and the .NET configuration file.
$built = Join-Path $projectDir 'bin\Release'
$files = Get-ChildItem $built -Recurse -File | Where-Object { $_.Extension -ne '.pdb' -and $_.Name -notlike '*.dll.config' }
if (-not ($files | Where-Object { $_.Name -eq "$modId.dll" })) { throw "Missing build output: $modId.dll" }

# The zip holds one folder named after the mod id, which is what the Mods folder expects.
# Entries are written one by one with forward slashes: Compress-Archive in Windows PowerShell
# writes backslashes, which do not extract as folders on Linux (Steam Deck) or macOS.
$dist = Join-Path $root 'dist'
New-Item -ItemType Directory -Force $dist | Out-Null
$zip = Join-Path $dist "$modId-$version.zip"
if (Test-Path $zip) { Remove-Item $zip -Force }

Add-Type -AssemblyName System.IO.Compression
Add-Type -AssemblyName System.IO.Compression.FileSystem
$archive = [System.IO.Compression.ZipFile]::Open($zip, [System.IO.Compression.ZipArchiveMode]::Create)
try {
    foreach ($file in $files) {
        $relative = $file.FullName.Substring($built.Length + 1).Replace('\', '/')
        [System.IO.Compression.ZipFileExtensions]::CreateEntryFromFile($archive, $file.FullName, "$modId/$relative") | Out-Null
    }
}
finally {
    $archive.Dispose()
}
Write-Host "Package: $zip"

if ($Install) {
    $mods = Join-Path $gamePath 'Mods'
    if (-not (Test-Path $mods)) { throw "Mods folder not found: $mods" }

    $target = Join-Path $mods $modId
    foreach ($file in $files) {
        $destination = Join-Path $target $file.FullName.Substring($built.Length + 1)
        New-Item -ItemType Directory -Force (Split-Path $destination) | Out-Null
        Copy-Item $file.FullName $destination -Force
    }
    Write-Host "Installed: $target"
}
