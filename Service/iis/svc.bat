@echo off

set siteName=%1
set appName=%2
set svcName=%3
set path=%4

set poolName=%appName%-%svcName%

echo ---------------------------
echo 安装[%svcName%]服务...

echo 删除旧应用、应用池
%SystemRoot%\system32\inetsrv\appcmd delete app /app.name:%siteName%/%appName%/%svcName%
%SystemRoot%\system32\inetsrv\appcmd delete apppool /apppool.name:%poolName%

echo 创建新应用池、应用
%SystemRoot%\system32\inetsrv\appcmd add apppool /name:%poolName% /managedRuntimeVersion: /startMode:AlwaysRunning /processModel.idleTimeout:00:00:00
%SystemRoot%\system32\inetsrv\appcmd add app /site.name:%siteName% /path:/%appName%/%svcName% /physicalPath:%path% /applicationpool:%poolName%