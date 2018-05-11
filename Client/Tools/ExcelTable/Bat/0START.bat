SET ExcelBatPath = ".\ClientCode\Tools\ExcelTable\Bat\"
SET Svnpath="C:\Program Files\TortoiseSVN\bin\TortoiseProc.exe"
%Svnpath% /command:update /path:"..\..\..\..\ConfigFile\Excel" /closeonend:0
cd %ExcelBatPath%
del /q ..\out\data\
del /q ..\out\config\
del /q ..\out\proto\
del /q ..\..\..\Assets\Resources\Databin\TableRes
del /q ..\..\..\Assets\Scripts\Proto\Table
del /q *.bytes
del /q *.proto
del /q *.txt
del /q *.pyc
python xls_deploy_tool.py
copy *.bytes ..\out\data\
copy *.proto ..\out\proto\

@for /f %%a in  ('dir /b ..\out\proto\*.proto')  do ..\..\Protocol\pb_csharp_gen\protogen.exe -i:"%%a" -o:"..\out\config\%%~na.cs" 

copy ..\out\config\*.cs  ..\..\..\Assets\Scripts\Proto\Table
copy ..\out\data\*.bytes  ..\..\..\Assets\Resources\Databin\TableRes

pause

