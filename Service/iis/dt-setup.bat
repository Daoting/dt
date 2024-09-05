@echo off

set site="Default Web Site"
set app="dt"
set cm_path="D:\Dt\Service\src\Cm"
set msg_path="D:\Dt\Service\src\Msg"
set fsm_path="D:\Dt\Service\src\Fsm"


call :setup_svc cm,%cm_path%
call :setup_svc msg,%msg_path%
call :setup_svc fsm,%fsm_path%

pause

:: ������ǰcmd������0
exit /b 0


::=============================================
:: ��װ������

:setup_svc
	set svc=%1
	set path=%2
	set app_svc=%app%-%svc%
	
	echo ---------------------------
	echo ��װ[%svc%]����...
	
	echo ɾ����Ӧ�á�Ӧ�ó�
	%SystemRoot%\system32\inetsrv\appcmd delete app /app.name:%site%/%app_svc%
	%SystemRoot%\system32\inetsrv\appcmd delete apppool /apppool.name:%app_svc%
	
	echo ������Ӧ�óء�Ӧ��
	%SystemRoot%\system32\inetsrv\appcmd add apppool /name:%app_svc% /managedRuntimeVersion: /startMode:AlwaysRunning /processModel.idleTimeout:00:00:00
	%SystemRoot%\system32\inetsrv\appcmd add app /site.name:%site% /path:/%app_svc% /physicalPath:%path% /applicationpool:%app_svc%

GOTO:EOF

pause