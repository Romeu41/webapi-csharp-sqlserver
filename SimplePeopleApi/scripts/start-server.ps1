# Start the API invisibly (no new visible console window)
# Usage: run this script from project root (or double-click in Explorer)
$projectPath = Join-Path $PSScriptRoot "..\SimplePeopleApi.csproj"
$logsDir = Join-Path $PSScriptRoot "..\logs"
if (-not (Test-Path $logsDir)) { New-Item -ItemType Directory -Path $logsDir | Out-Null }
 $logFile = Join-Path $logsDir "server_out.log"
 $errFile = Join-Path $logsDir "server_err.log"

$dotnet = "dotnet"
$dotnetArgs = "run --project `"$projectPath`" --urls `"http://localhost:5002`""

Start-Process -FilePath $dotnet -ArgumentList $dotnetArgs -WorkingDirectory (Split-Path $projectPath) -WindowStyle Hidden -RedirectStandardOutput $logFile -RedirectStandardError $errFile
Write-Host "Started SimplePeopleApi (hidden). Logs: $logFile (out) and $errFile (err)"