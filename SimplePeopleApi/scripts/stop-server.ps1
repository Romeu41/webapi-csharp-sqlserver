# Stop processes whose command line contains SimplePeopleApi (PowerShell)
Get-CimInstance Win32_Process |
  Where-Object { $_.CommandLine -and $_.CommandLine -match 'SimplePeopleApi' } |
  ForEach-Object {
    Write-Host "Stopping PID $($_.ProcessId): $($_.CommandLine)"
    try { Stop-Process -Id $_.ProcessId -Force -ErrorAction Stop }
    catch { Write-Warning "Failed to stop $($_.ProcessId): $($_.Exception.Message)" }
  }
