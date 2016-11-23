Import-Module WebAdministration
$data = Get-ChildItem -Path IIS:\AppPools | where-object {$_.Name -notLike ".Net*"} | select name 
If ($data){

$data = Get-ChildItem -Path IIS:\AppPools | where-object {$_.Name -notLike ".Net*"} | select name 
$name = $data.name

Set-ItemProperty iis:\apppools\$name -name processModel -value @{userName="ppdigbe\SVC_PAPI_DEDS";password="_ewESp4cuWre";identitytype=3}

}
else{

}