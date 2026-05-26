Write-Host "========================================" -ForegroundColor Cyan
Write-Host "EPMS KPI Fix Migration Script" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

$scriptPath = Join-Path $PSScriptRoot "EPMS.Infrastructure\SqlScripts\CompleteKpiFixMigration.sql"

if (-not (Test-Path $scriptPath)) {
    Write-Host "ERROR: Migration script not found at: $scriptPath" -ForegroundColor Red
    exit 1
}

Write-Host "Migration script found:" -ForegroundColor Green
Write-Host "  $scriptPath" -ForegroundColor Gray
Write-Host ""

Write-Host "Please run the migration script using one of these methods:" -ForegroundColor Yellow
Write-Host ""
Write-Host "Method 1: SQL Server Management Studio (Recommended)" -ForegroundColor White
Write-Host "  1. Open SQL Server Management Studio" -ForegroundColor Gray
Write-Host "  2. Connect to your SQL Server" -ForegroundColor Gray
Write-Host "  3. Open: $scriptPath" -ForegroundColor Gray
Write-Host "  4. Click Execute (F5)" -ForegroundColor Gray
Write-Host ""
Write-Host "Method 2: sqlcmd (if available)" -ForegroundColor White
Write-Host "  Run this command:" -ForegroundColor Gray
Write-Host ""
Write-Host "  sqlcmd -S localhost -d EmployeePerformance -i `"$scriptPath`"" -ForegroundColor Cyan
Write-Host ""
Write-Host "  (Replace localhost with your server name if needed)" -ForegroundColor Gray
Write-Host ""
Write-Host "Press any key to exit..."
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown)
