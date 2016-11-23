Import-Module WebAdministration

Set-ItemProperty "IIS:\AppPools\.NET v4.5 Classic" -name processModel -value @{userName="user_name";password="password";identitytype=3}

Set-ItemProperty "IIS:\AppPools\.NET v4.5" -name processModel -value @{userName="user_name";password="password";identitytype=3}

