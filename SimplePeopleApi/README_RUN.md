Start / Stop helper for development

This repository includes small helper scripts to start the API without blocking or closing your current terminals.

Start (detached, writes logs):
- On Windows, double-click or run from PowerShell/CMD:
  scripts\start-server.bat
  This will open a new console window running `dotnet run` and redirect output to `logs\server.log`.

Start invisibly (no visible window):
- You can run the PowerShell helper which starts the server in the background without opening a visible console:
  - PowerShell: scripts\start-server.ps1
  - This starts `dotnet run` hidden and redirects output to `logs\server.log`.

Stop (best-effort):
- To stop the server started by the script, run:
  scripts\stop-server.bat
  This tries to find the PID listening on port 5002 and kill it.

Direct run (foreground):
- If you prefer to run in the current terminal and see live logs, run:
  dotnet run --project "C:\projeto\SimplePeopleApi\SimplePeopleApi.csproj" --urls "http://localhost:5002"
  Leave the terminal open while testing (Ctrl+C to stop).

Where to look for logs:
- logs\server.log (created by start-server.bat) contains stdout/stderr from the run.
- logs\shutdown.txt contains any uncaught exceptions or lifetime events logged by the app.

If start-server.bat doesn't open a window (corporate policies/antivirus), or you prefer no visible window, use `scripts\start-server.ps1` (PowerShell). If neither works, run the foreground command and leave the terminal open.

If you want me to change the start script to run with PowerShell's Start-Job / Register-ScheduledTask or create a Windows Service, tell me which option you prefer.