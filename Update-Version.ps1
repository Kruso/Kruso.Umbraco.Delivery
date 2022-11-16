############################################################
#Project : Reading XML files with PowerShell
#Developer : Thiyagu S (dotnet-helpers.com)
#Tools : PowerShell 5.1.15063.1155 
#E-Mail : mail2thiyaguji@gmail.com 
############################################################<br />
param (
    # XML file
    [string]$proj
)

[XML]$projFile = [xml](Get-Content $proj)
 
foreach($propGroup in $projFile.Project.PropertyGroup) {
  if ($propGroup.Version) {
    Write-Host "Old version :" $propGroup.Version

    $fileVerParts = $propGroup.FileVersion.split(".")
    $fileVerMajor = $fileVerParts[0] -as [int]
    $fileVerMinor = $fileVerParts[1] -as [int]
    $fileVerPatch = $fileVerParts[2] -as [int]
    $fileVerBuild = ($fileVerParts[3] -as [int]) + 1

    $newVersion = "$fileVerMajor.$fileVerMinor.$fileVerBuild"
    Write-Host "New Version: " $newVersion
    $propGroup.Version = $newVersion

    $newFileVersion = "$fileVerMajor.$fileVerMinor.$fileVerPatch.$fileVerBuild"
    Write-Host "New FileVersion: " $newFileVersion
    $propGroup.FileVersion = $newFileVersion
    Write-Host "New AssemblyVersion: " $newFileVersion
    $propGroup.AssemblyVersion = $newFileVersion

    $projFile.Save((Resolve-Path $proj))

    break
  }
}