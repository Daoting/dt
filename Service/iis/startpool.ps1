cd C:\Windows\System32\inetsrv\
.\appcmd.exe list apppool | Select-String -Pattern '(?<=").*?(?=")' | ForEach{ '/apppool.name:"{0}"' -f $_.Matches[0]} | ForEach { .\appcmd.exe start apppool $_}