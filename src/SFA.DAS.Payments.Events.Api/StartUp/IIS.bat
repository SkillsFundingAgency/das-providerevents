@echo off 
powershell -command "Set-ExecutionPolicy Unrestricted" 
powershell .\IIS.ps1 
exit /b 0