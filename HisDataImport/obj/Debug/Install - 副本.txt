pushd %~dp0
%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\installutil.exe HisDataImport.exe
Net Start HisDataImport
sc config HisDataImport start= auto
Pause