@echo off

set siteName=%1
set appName=%2

echo ---------------------------
echo ��װ[%appName%]...

echo ɾ����Ӧ�á�Ӧ�ó�
%SystemRoot%\system32\inetsrv\appcmd delete app /app.name:%siteName%/%appName%

echo ������Ӧ�óء�Ӧ��
%SystemRoot%\system32\inetsrv\appcmd add app /site.name:%siteName% /path:/%appName% /physicalPath:"C:\inetpub\wwwroot"