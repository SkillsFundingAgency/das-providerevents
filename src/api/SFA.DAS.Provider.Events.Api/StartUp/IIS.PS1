function Install-MSIFile {

[CmdletBinding()]
 Param(
  [parameter(mandatory=$true,ValueFromPipeline=$true,ValueFromPipelinebyPropertyName=$true)]
        [ValidateNotNullorEmpty()]
        [string]$msiFile,

        [parameter()]
        [ValidateNotNullorEmpty()]
        [string]$targetDir
 )
if (!(Test-Path $msiFile)){
    throw "Path to the MSI File $($msiFile) is invalid. Please supply a valid MSI file"
}
$arguments = @(
    "/i"
    "`"$msiFile`""
    "/qn"
)
if ($targetDir){
    if (!(Test-Path $targetDir)){
        throw "Path to the Installation Directory $($targetDir) is invalid. Please supply a valid installation directory"
    }
    $arguments += "INSTALLDIR=`"$targetDir`""
}
Write-Verbose "Installing $msiFile....."
$process = Start-Process -FilePath msiexec.exe -ArgumentList $arguments -Wait -PassThru
if ($process.ExitCode -eq 0){
    Write-Verbose "$msiFile has been successfully installed"
}
else {
    Write-Verbose "installer exit code  $($process.ExitCode) for file  $($msifile)"
}
}

#Logging in the first instance#
Import-Module -Name "..\..\approot\bin\NLog.dll"
Import-Module -Name "..\..\approot\bin\Newtonsoft.Json.dll"
Import-Module -Name "..\..\approot\bin\Microsoft.WindowsAzure.Configuration.dll"
Import-Module -Name "..\..\approot\bin\Microsoft.WindowsAzure.Storage.dll"


$Azure="Azure"


while(!($myWeb = Get-Website -name "SFA.DAS.Provider.Events.Api*")){
    Write-Host "Website not installed. Waiting 30 seconds..."
	$logger.Error("Website not deployed waiting 30 seconds")
    Start-Sleep 30
}

if(Get-Module -ListAvailable | Where-Object{ $_.Name -eq $Azure }) 
{  
$logger.Error("Azure MSI Installed Getting AppPool Data")
[Reflection.Assembly]::LoadWithPartialName("Microsoft.WindowsAzure.ServiceRuntime")

$Username = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("dmuser")
$Password = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("dmpwd")


Import-Module WebAdministration -Force

$IISName= get-childitem -path iis:\apppools\ | where Name -NotLike ".Net*" | select name
$IIS=$IISName.name
$logger.Error("Setting App Pool Identity $Username, $IIS")

Try {Set-ItemProperty iis:\apppools\$IIS -name processModel -value @{userName="$UserName";password="$Password";identitytype=3}}
catch {
$logger.Error("There has been an error while setting the APP POOL identity")
break
}

}  
else  
{  

$logger.Error("Installing MSI")
 "StartUp\Powershell.msi" | Install-MSIFile
}

