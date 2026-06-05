@echo off
REM Stop all backend services started from the sos-api folder via PowerShell script.
REM Usage: stop-all

set SCRIPT_DIR=%~dp0
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%stop-all.ps1"
