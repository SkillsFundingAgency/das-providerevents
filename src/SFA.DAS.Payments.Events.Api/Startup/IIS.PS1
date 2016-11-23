Import-Module WebAdministration
$appPool = Get-Website | Select-Object applicationPool
$name = $appPool.applicationPool
Set-ItemProperty -Path "IIS:\AppPools\$name" -Name processModel.userName -Value "username"
Set-ItemProperty -Path "IIS:\AppPools\$name" -Name processModel.password -Value "password"
Set-ItemProperty -Path "IIS:\AppPools\$name" -Name processModel.identityType -Value 3