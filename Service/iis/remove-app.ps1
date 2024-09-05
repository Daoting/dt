Param([string] $siteName, [string] $appName)
$name = $appName + "-*"
$path = $siteName + "/" + $name

cd C:\Windows\System32\inetsrv\
.\appcmd.exe list app | Select-String -Pattern '(?<=").*?(?=")' | where{$_.Matches[0] -like $path} | ForEach{ '/app.name:"{0}"' -f $_.Matches[0]} | ForEach{ .\appcmd.exe delete app $_}
.\appcmd.exe list apppool | Select-String -Pattern '(?<=").*?(?=")' | where{$_.Matches[0] -like $name} | ForEach{ '/apppool.name:"{0}"' -f $_.Matches[0]} | ForEach{ .\appcmd.exe delete apppool $_}