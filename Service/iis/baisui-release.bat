@echo off

set site="Default Web Site"
set app=bs

echo 创建一级目录
%SystemRoot%\system32\inetsrv\appcmd add vdir /app.name:%site%/ /path:/%app% /physicalPath:"C:\inetpub\wwwroot"

call %~dp0\svc.bat %site% %app% cm "D:\Dt\Service\publish\cm"
call %~dp0\svc.bat %site% %app% msg "D:\Dt\Service\publish\msg"
call %~dp0\svc.bat %site% %app% fsm "D:\Dt\Service\publish\fsm"
call %~dp0\svc.bat %site% %app% pub "D:\Dt\Service\publish\pub"

pause