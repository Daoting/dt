@echo off

set siteName=%1
set appName=%2

echo ---------------------------
echo 安装[%appName%]...

echo 删除旧应用、应用池
%SystemRoot%\system32\inetsrv\appcmd delete app /app.name:%siteName%/%appName%

echo 创建新应用池、应用
%SystemRoot%\system32\inetsrv\appcmd add app /site.name:%siteName% /path:/%appName% /physicalPath:"C:\inetpub\wwwroot"