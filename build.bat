@echo off

set gac=C:\Windows\Microsoft.NET\assembly\GAC_MSIL
set csc=@c:\Windows\Microsoft.NET\Framework\v4.0.30319\csc.exe

set acm=Altiris.Common
set acc=Altiris.AssetContractCommon
set ans=Altiris.NS
set ars=Altiris.Resource
set adb=Altiris.Database
set asi=Altiris.NS.StandardItems
set ats=Altiris.TaskManagement
set atc=Altiris.TaskManagement.Common
set apm=Altiris.PatchManagementCore
set apw=Altiris.PatchManagementCore.Web
set adn=Altiris.DotNetLib
set airm=Altiris.InventoryRuleManagement
set asm=Altiris.SoftwareManagement
set arh=Altiris.NS.Replication.Hierarchy

set no_obs=find /v /i "is obsolete"
set no_pol=find /v /i "need to supply runtime policy"
set no_prv=find /v /i "related to previous warning"


if "%1"=="7.1" goto build-7.1
if "%1"=="7.6" goto build-7.6

:build-7.5
set ver1=7.5.3153.0__d516cb311cfb6e4f
set ver4=7.5.3125.0__d516cb311cfb6e4f
set ver2=7.5.3219.0__d516cb311cfb6e4f
set ver3=7.5.3153.0__99b1e4cc0d03f223

set gac=C:\Windows\Assembly\GAC_MSIL

set id=7.5
goto build

:build-7.6

set ver1=v4.0_7.6.1383.0__d516cb311cfb6e4f
set ver2=v4.0_7.6.1395.0__d516cb311cfb6e4f
set ver3=v4.0_7.6.1383.0__99b1e4cc0d03f223

set id=7.6
goto build

:build-7.1
set ver1=7.1.8400.0__d516cb311cfb6e4f
set ver2=7.1.7858.0__d516cb311cfb6e4f
set ver3=7.1.8400.0__99b1e4cc0d03f223

set gac=C:\Windows\Assembly\GAC_MSIL
set csc=@c:\Windows\Microsoft.NET\Framework\v2.0.50727\csc.exe

set id=7.1
goto build


:build
cmd /c %csc% /reference:%gac%\%arh%\%ver1%\%arh%.dll /reference:%gac%\%airm%\%ver1%\%airm%.dll /reference:%gac%\%acm%\%ver1%\%acm%.dll /reference:%gac%\%acc%\%ver2%\%acc%.dll /reference:%gac%\%ans%\%ver1%\%ans%.dll /reference:%gac%\%ars%\%ver1%\%ars%.dll /reference:%gac%\%adb%\%ver1%\%adb%.dll /reference:%gac%\%asi%\%ver1%\%asi%.dll /reference:%gac%\%ats%\%ver3%\%ats%.dll /reference:%gac%\%apm%\%ver2%\%apm%.dll /reference:%gac%\%atc%\%ver3%\%atc%.dll /reference:%gac%\%adn%\%ver3%\%adn%.dll  /out:importexport2.exe *.cs