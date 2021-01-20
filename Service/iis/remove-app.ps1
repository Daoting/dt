Param([string] $siteName, [string] $appName)
$pool = $appName + "-*"
$path = $siteName + "/" + $appName + "/*"
$vdir = $siteName + "/" + $appName

cd C:\Windows\System32\inetsrv\
#.\appcmd.exe list app | Select-String -Pattern '(?<=").*?(?=")' | where{$_.Matches[0] -like $path} | ForEach{ Write-Host $_.Matches[0]}
.\appcmd.exe list app | Select-String -Pattern '(?<=").*?(?=")' | where{$_.Matches[0] -like $path} | ForEach{ '/app.name:"{0}"' -f $_.Matches[0]} | ForEach{ .\appcmd.exe delete app $_}
.\appcmd.exe delete vdir $vdir
.\appcmd.exe list apppool | Select-String -Pattern '(?<=").*?(?=")' | where{$_.Matches[0] -like $pool} | ForEach{ '/apppool.name:"{0}"' -f $_.Matches[0]} | ForEach{ .\appcmd.exe delete apppool $_}