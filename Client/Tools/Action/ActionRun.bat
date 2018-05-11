SET GitPath="..\..\..\..\..\TowerGame"
cd %GitPath%
git pull 
cd ".\ClientCode\Tools\Action\"
copy ..\..\Assets\Scripts\Proto\ActionEvent\LEventData.cs  ..\..\Assets\Scripts\Proto\
del /q .\Out\
del /q .\Proto\
del /q ..\..\Assets\Scripts\Proto\ActionEvent\
copy ..\..\..\ConfigFile\Action\*.proto  .\Proto\
@for /f %%a in  ('dir /b /s .\Proto\*.proto')  do ..\Protocol\pb_csharp_gen\protogen.exe -i:"%%a" -o:".\Out\%%~na.cs" 
del /q .\Out\LEventData.cs
copy .\Out\*.cs  ..\..\Assets\Scripts\Proto\ActionEvent
copy ..\..\Assets\Scripts\Proto\LEventData.cs ..\..\Assets\Scripts\Proto\ActionEvent\
del /q ..\..\Assets\Scripts\Proto\LEventData.cs
pause

