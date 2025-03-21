@echo off

:: path of the script dir
:: cd %~dp0

set workingDir=%1
set workingDir=%workingDir:~1,-1%
:: echo %workingDir%
cd %workingDir%

:: removing first and last "
set gitMessage=%2
set gitMessage=%gitMessage:~1,-1%

git commit -am "mission planner fastParamPlugin: %gitMessage%"

:: pause