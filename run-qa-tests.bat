@echo off
setlocal

powershell -NoProfile -ExecutionPolicy Bypass -File "%~dp0run-qa-tests.ps1"
set "QA_EXIT_CODE=%ERRORLEVEL%"

echo.
echo ========================================
echo La ejecucion QA finalizo con codigo: %QA_EXIT_CODE%
echo Revise el resumen anterior y tome las capturas necesarias.
echo Presione una tecla para cerrar esta ventana...
echo ========================================
pause > nul

exit /b %QA_EXIT_CODE%
