:: @echo off
:: path of the script dir
cd %~dp0

echo %1

git config --global user.email "jair.fehlauer@gmail.com"
git config --global user.name "Jair-F"

git commit -am "mission planner fastParamPlugin: %1 added new params"

pause