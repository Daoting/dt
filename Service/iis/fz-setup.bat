@echo off

set site="Default Web Site"
set app=fz

echo 创建一级目录
%SystemRoot%\system32\inetsrv\appcmd add vdir /app.name:%site%/ /path:/%app% /physicalPath:"C:\inetpub\wwwroot"

call %~dp0\svc.bat %site% %app% cm "D:\Dt\Service\src\Dt.Cm"
call %~dp0\svc.bat %site% %app% msg "D:\Dt\Service\src\Dt.Msg"
call %~dp0\svc.bat %site% %app% fsm "D:\Dt\Service\src\Dt.Fsm"
call %~dp0\svc.bat %site% %app% pub "D:\Dt\Service\src\Dt.Pub"
call %~dp0\svc.bat %site% %app% ui "D:\Dt\Client\Dt.Shell.Wasm"

pause