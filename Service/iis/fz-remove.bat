@echo off
powershell.exe -command "&{%~dp0remove-app.ps1 'Default Web Site' fz}"
pause