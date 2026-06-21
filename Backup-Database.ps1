# MES Database Backup Script

$mysqldump = "C:\Program Files\MySQL\MySQL Server 9.7\bin\mysqldump.exe"
$db = "mes_prod"
$user = "root"
$pass = "MyNewPass123!"
$backupDir = Join-Path $PSScriptRoot "backups"
$timestamp = Get-Date -Format "yyyyMMdd_HHmmss"
$backupFile = Join-Path $backupDir "mes_prod_$timestamp.sql"

# Create backup directory if not exists
if (-not (Test-Path $backupDir)) {
    New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
}

Write-Host "Starting database backup..."
Write-Host "Database: $db"
Write-Host "Backup file: $backupFile"

& $mysqldump --user=$user --password=$pass --databases $db --routines --triggers --single-transaction --quick --lock-tables=false | Out-File -FilePath $backupFile -Encoding utf8

if ($LASTEXITCODE -eq 0) {
    $fileSize = (Get-Item $backupFile).Length / 1MB
    Write-Host "Backup completed successfully!"
    Write-Host "File: $backupFile"
    Write-Host "Size: $([math]::Round($fileSize, 2)) MB"
    
    # Also create latest symlink
    $latestLink = Join-Path $backupDir "mes_prod_latest.sql"
    if (Test-Path $latestLink) { Remove-Item $latestLink }
    Copy-Item $backupFile $latestLink
    Write-Host "Latest backup: $latestLink"
} else {
    Write-Host "Backup failed with exit code: $LASTEXITCODE" -ForegroundColor Red
    exit 1
}
