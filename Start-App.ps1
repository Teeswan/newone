# Stop existing processes on ports 5111 and 5052
Get-NetTCPConnection -LocalPort 5111, 5052 -ErrorAction SilentlyContinue | ForEach-Object {
    try {
        Stop-Process -Id $_.OwningProcess -Force -ErrorAction SilentlyContinue
        Write-Host "Stopped process on port $($_.LocalPort)"
    } catch {}
}

Write-Host "Starting API..." -ForegroundColor Cyan
Start-Process dotnet -ArgumentList "run --project EPMS.Api/EPMS.Api.csproj --launch-profile http" -WindowStyle Hidden

Write-Host "Waiting for API to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

Write-Host "Starting Blazor App..." -ForegroundColor Cyan
Start-Process dotnet -ArgumentList "run --project EPMS.Blazor/EPMS.Blazor.csproj --launch-profile http" -WindowStyle Hidden

Write-Host "Waiting for Blazor App to start..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

Write-Host "Opening Blazor App in browser..." -ForegroundColor Green
Start-Process "http://localhost:5052"

Write-Host "Application is running!" -ForegroundColor Green
Write-Host "API: http://localhost:5111/swagger"
Write-Host "Blazor: http://localhost:5052"
