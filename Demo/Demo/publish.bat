@REM VS开发环境cmd，Developer Command Prompt for VS 2022
call "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\Tools\VsDevCmd.bat"

@REM 打包并输出
msbuild /r /p:TargetFramework=net9.0-windows10.0.19041 /p:Configuration=Release /p:Platform=x64 /p:GenerateAppxPackageOnBuild=true /p:AppxBundle=Never /p:UapAppxPackageBuildMode=Sideloading /p:AppxPackageDir="D:/Dt/Packages/" /p:AppxPackageSigningEnabled=true

pause