@echo off

set site="Default Web Site"
set app=baisui

call %~dp0\app.bat %site% %app%
call %~dp0\svc.bat %site% %app% cm "D:\Dt\Service\src\Dt.Cm\bin\Debug\net5.0"
call %~dp0\svc.bat %site% %app% msg "D:\Dt\Service\src\Dt.Msg\bin\Debug\net5.0"
call %~dp0\svc.bat %site% %app% fsm "D:\Dt\Service\src\Dt.Fsm\bin\Debug\net5.0"
call %~dp0\svc.bat %site% %app% pub "D:\Dt\Service\src\Dt.Pub\bin\Debug\net5.0"

pause