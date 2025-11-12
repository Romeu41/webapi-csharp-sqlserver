@echo off
REM Safer stop: stop only processes whose commandline contains 'SimplePeopleApi'
REM This uses PowerShell to inspect process command lines and stop matching processes.
powershell -NoProfile -Command "Get-CimInstance Win32_Process | Where-Object { $_.CommandLine -and $_.CommandLine -match 'SimplePeopleApi' } | ForEach-Object { Write-Host \"Stopping PID $($_.ProcessId): $($_.CommandLine)\"; try { Stop-Process -Id $_.ProcessId -Force -ErrorAction Stop; } catch { Write-Host \"Failed to stop $($_.ProcessId): $($_.Exception.Message)\" } }"
echo Done.
exit /B 0
