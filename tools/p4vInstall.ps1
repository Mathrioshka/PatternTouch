#param($installPath, $toolsPath, $package, $project)
$toolsPath = split-path -parent $MyInvocation.MyCommand.Definition


Import-Module BitsTransfer
[System.Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.FileSystem')

$parent = Split-Path -parent $toolsPath
$tempDir = Join-Path $parent "temp"

if (-not (Test-Path $tempDir)) {
    New-Item -Path $tempDir -ItemType directory
}

Start-BitsTransfer "http://vvvv.org/sites/all/modules/general/pubdlcnt/pubdlcnt.php?file=http://vvvv.org/sites/default/files/uploads/patterntouch.0.1.zip" $tempDir
$archive = Get-ChildItem $tempDir -Filter *.zip -Name
$archive = Join-Path $tempDir $archive

[System.IO.Compression.ZipFile]::ExtractToDirectory($archive, $parent)

Remove-Item $tempDir -Recurse -Force