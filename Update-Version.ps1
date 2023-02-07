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

    $fileVerParts = $propGroup.Version.split(".")
    $fileVerMajor = $fileVerParts[0] -as [int]
    $fileVerMinor = $fileVerParts[1] -as [int]
    $fileVerOldPatch = $fileVerParts[2] -as [string]

    $fileVerPatch = 0;
    $fileVerPreview = 0;
    if ($fileVerOldPatch.Contains("-preview")) {
      $parts = $fileVerOldPatch.Replace("-preview", ".").Split(".")
      $fileVerPatch = $parts[0] -as [int]
      $fileVerPreview = $parts[1] -as [int]
    } else {
      $fileVerPatch = ([int]$fileVerOldPatch) + 1
    }
    $fileVerPreview = $fileVerPreview + 1

    $newVersion = "$fileVerMajor.$fileVerMinor.$fileVerPatch-preview$fileVerPreview"
    Write-Host "New Version: " $newVersion
    $propGroup.Version = $newVersion

    $projFile.Save((Resolve-Path $proj))

    Write-Output "::set-output name=newVersion::$($newVersion)"

    break
  }
}