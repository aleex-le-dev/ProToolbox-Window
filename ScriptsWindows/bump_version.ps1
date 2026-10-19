# Bump automatique de version (+0.0.1)
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$csprojPath = Join-Path $scriptDir "ScriptsWindows\ScriptsWindows.csproj"

if (-not (Test-Path $csprojPath)) {
    Write-Error "Fichier csproj introuvable : $csprojPath"
    exit 1
}

$content = Get-Content -Path $csprojPath -Raw

$currentVersion = "4.2.2"
if ($content -match '<Version>(.*?)</Version>') {
    $currentVersion = $Matches[1].Trim()
}

# Parser la version (ex: 4.2.2)
$parts = $currentVersion.Split('.')
$major = 4
$minor = 2
$patch = 0

if ($parts.Length -ge 1) { [int]::TryParse($parts[0], [ref]$major) | Out-Null }
if ($parts.Length -ge 2) { [int]::TryParse($parts[1], [ref]$minor) | Out-Null }
if ($parts.Length -ge 3) { [int]::TryParse($parts[2], [ref]$patch) | Out-Null }

$newPatch = $patch + 1
$newVersion = "$major.$minor.$newPatch"

# Remplacement dans le csproj
$newContent = [System.Text.RegularExpressions.Regex]::Replace(
    $content,
    '<Version>.*?</Version>',
    "<Version>$newVersion</Version>"
)

[System.IO.File]::WriteAllText($csprojPath, $newContent, [System.Text.Encoding]::UTF8)

# Sortie pour le script batch appelant : ANCIENNE;NOUVELLE
Write-Output "$currentVersion;$newVersion"
