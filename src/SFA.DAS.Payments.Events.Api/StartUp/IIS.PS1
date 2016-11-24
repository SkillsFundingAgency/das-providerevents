while(!($myWeb = Get-Website -name "SFA.DAS.Payments.Events.Api*")){
    Write-Host "Website not installed. Waiting 30 seconds..."
    Start-Sleep 30
}

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

$Connection=$entities.Properties
$Connection1=$connection.Values
$ConnectionString=$Connection1.PropertyAsObject

Import-Module WebAdministration
$data = Get-ChildItem -Path IIS:\AppPools | where-object {$_.Name -notLike ".Net*"} | select name 
$name = $data.name
Set-ItemProperty $ConnectionString
