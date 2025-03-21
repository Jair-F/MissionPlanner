:: @echo off
:: path of the script dir
cd %~dp0

echo %1
:: removing first and last "
set param1=%1
set param1=%param1:~1,-1%
echo %param1%

git config --global user.email "jair.fehlauer@gmail.com"
git config --global user.name "Jair-F"

git commit -am "mission planner fastParamPlugin: %param1%"
