PowerShell.exe -ExecutionPolicy Unrestricted .\IIS.ps1 >> "%TEMP%\StartupLog.txt" 2>&1

REM   If an error occurred, return the errorlevel.
EXIT /B %errorlevel%