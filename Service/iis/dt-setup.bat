chcp 65001
@echo off

set site="Default Web Site"
set app="dt"
set cm_path="D:\Dt\Master\Service\src\Test\Cm"
set msg_path="D:\Dt\Master\Service\src\Test\Msg"
set fsm_path="D:\Dt\Master\Service\src\Test\Fsm"
set app_path="D:\Dt\Master\Service\src\Test\App"
set da_path="D:\Dt\Master\Service\src\Test\Da"

call :setup_svc cm,%cm_path%
call :setup_svc msg,%msg_path%
call :setup_svc fsm,%fsm_path%
call :setup_svc app,%app_path%
call :setup_svc da,%da_path%

pause

exit /b 0


::=============================================
:: 函数

:setup_svc
	set svc=%1
	set path=%2
	set app_svc=%app%-%svc%
	
	echo ---------------------------
	echo 安装 [%svc%] 应用...
	
	%SystemRoot%\system32\inetsrv\appcmd delete app /app.name:%site%/%app_svc%
	%SystemRoot%\system32\inetsrv\appcmd delete apppool /apppool.name:%app_svc%
	
	%SystemRoot%\system32\inetsrv\appcmd add apppool /name:%app_svc% /managedRuntimeVersion: /startMode:AlwaysRunning /processModel.idleTimeout:00:00:00
	%SystemRoot%\system32\inetsrv\appcmd add app /site.name:%site% /path:/%app_svc% /physicalPath:%path% /applicationpool:%app_svc%

GOTO:EOF

pause