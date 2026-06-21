@echo off
"C:\Program Files\MySQL\MySQL Server 9.7\bin\mysql.exe" -h localhost -u root -pMyNewPass123! mes_prod --default-character-set=utf8mb4 < e:\AiProj\MES_NEW\docs\sql\migrations\V3.0.0__api_layer_tables.sql
echo Exit code: %errorlevel%
pause
