@echo off
REM Start the API in a new window and redirect stdout/stderr to logs\server.log
REM Usage: double-click or run from Powershell/CMD: scripts\start-server.bat
if not exist ..\logs mkdir ..\logs
ncd ..\
start "SimplePeopleApi" cmd /c "dotnet run --project "%cd%\SimplePeopleApi.csproj" --urls "http://localhost:5002" > "%cd%\logs\server.log" 2>&1"
exit /B 0
