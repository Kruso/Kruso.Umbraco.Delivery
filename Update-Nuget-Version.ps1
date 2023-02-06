############################################################
#Project : Reading XML files with PowerShell
#Developer : Thiyagu S (dotnet-helpers.com)
#Tools : PowerShell 5.1.15063.1155 
#E-Mail : mail2thiyaguji@gmail.com 
############################################################<br />
param (
    # XML file
    [string]$proj,
    [string]$ver
)

[XML]$projFile = [xml](Get-Content $proj)
 
foreach($propGroup in $projFile.Project.PropertyGroup) {
  if ($propGroup.Version) {
    Write-Host "Old version :" $propGroup.Version
    Write-Host "New Version: " $ver

    $propGroup.Version = $ver
    $projFile.Save((Resolve-Path $proj))

    break
  }
}