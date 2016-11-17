@echo off
set csc=@c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe

if "%1"=="7.1" goto build-7.1
if "%1"=="7.5" goto build-7.5
if "%1"=="7.6" goto build-7.6

:build 8.0
set ans=C:\Windows\Microsoft.Net\assembly\GAC_MSIL\Altiris.NS\v4.0_8.0.3396.0__d516cb311cfb6e4f\Altiris.NS.dll
set id=8.0
goto build

:build-7.6
set ans=C:\Windows\Assembly\GAC_MSIL\Altiris.NS\v4.0_7.6.1383.0__d516cb311cfb6e4f\Altiris.NS.dll
set id=7.6
goto build

:build-7.5
set ans=C:\Windows\Assembly\GAC_MSIL\Altiris.NS\7.5.3153.0__d516cb311cfb6e4f\Altiris.NS.dll
set id=7.5
goto build

:build-7.1
set ans=C:\Windows\Assembly\GAC_MSIL\Altiris.NS\7.1.8400.0__d516cb311cfb6e4f\Altiris.NS.dll
set csc=@c:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe
set id=7.1
goto build:


build
cmd /c %csc% /reference:%ans% /out:importexport-%id%.exe *.cs
