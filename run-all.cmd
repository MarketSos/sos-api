@echo off
REM Run all backend services from the sos-api folder via PowerShell script.
REM Usage: run-all

set SCRIPT_DIR=%~dp0
powershell.exe -NoProfile -ExecutionPolicy Bypass -File "%SCRIPT_DIR%run-all.ps1"
