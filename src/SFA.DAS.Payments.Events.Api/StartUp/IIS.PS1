while(!($myWeb = Get-Website -name "SFA.DAS.Payments.Events.Api*")){
    Write-Host "Website not installed. Waiting 30 seconds..."
    Start-Sleep 30
}

Import-Module WebAdministration
$data = Get-ChildItem -Path IIS:\AppPools | where-object {$_.Name -notLike ".Net*"} | select name 
$name = $data.name
Set-ItemProperty iis:\apppools\$name -name processModel -value @{userName="__ServiceAccount";password="__ServiceAccountPassword__";identitytype=3}
