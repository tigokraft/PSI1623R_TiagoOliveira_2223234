@echo off
chcp 65001 >nul

:loop
cls
echo 🔧 Cleaning...
dotnet clean

echo 🚀 Starting FinSync API...
dotnet run

echo 🔁 Restart requested. Looping...
goto loop