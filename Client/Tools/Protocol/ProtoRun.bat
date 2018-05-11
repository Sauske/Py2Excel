::SET GitPath="..\..\..\..\TowerGame"
::cd %GitPath%
::git pull 
SET NetBatPath = ".\GameFramework\Tools\Protocol\"
cd %NetBatPath%
del /q .\Out\
del /q .\Proto\
del /q ..\..\Assets\Scripts\Proto\NetProto\
copy ..\..\..\ConfigFile\Protocal\*.proto  .\Proto\
@for /f %%a in  ('dir /b /s .\Proto\*.proto')  do ..\Protocol\pb_csharp_gen\protogen.exe -i:"%%a" -o:".\Out\%%~na.cs" 
copy .\Out\*.cs  ..\..\Assets\Scripts\Proto\NetProto
pause

