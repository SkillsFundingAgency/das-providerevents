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
$name="Azure"

while(!($myWeb = Get-Website -name "SFA.DAS.Payments.Events.Api*")){
    Write-Host "Website not installed. Waiting 30 seconds..."
    Start-Sleep 15
}

if(Get-Module -ListAvailable | Where-Object{ $_.Name -eq $name }) 
{  
[Reflection.Assembly]::LoadWithPartialName("Microsoft.WindowsAzure.ServiceRuntime")

$ConfigurationStorageConnectionString = [Microsoft.WindowsAzure.ServiceRuntime.RoleEnvironment]::GetConfigurationSettingValue("ConfigurationStorageConnectionString")

#Query
$Ctx = New-AzureStorageContext -ConnectionString $ConfigurationStorageConnectionString
$TableName = "Configuration"

$table = Get-AzureStorageTable –Name $TableName -Context $Ctx
$query = New-Object Microsoft.WindowsAzure.Storage.Table.TableQuery

#Define columns to select.
$list = New-Object System.Collections.Generic.List[string]
$list.Add("PartitionKey")
$list.Add("RowKey")
$list.Add("Data")

$query.FilterString =  "RowKey eq 'PAPI App Pool Connection'"
$query.SelectColumns = $list

$entities = $table.CloudTable.ExecuteQuery($query)

$Data1=$entities.Properties
$Data2=$Data1.Values
$String=$Data2.PropertyAsObject

write-host "$String" 

$UserName,$Password = $string.split(' ',2)

Set-ItemProperty iis:\apppools\$name -name processModel -value @{userName="$UserName";password="$Password";identitytype=3}

}  
else  
{  
 "Powershell.msi" | Install-MSIFile
}

