@echo off
set "current_dir=%~dp0"

set "parent_dir=%current_dir:~0,-1%"
cd %parent_dir%
set "parent_dir=%cd%"


set "parent_dir2=%parent_dir%\.."
cd %parent_dir2%
set "parent_dir2=%cd%"

set "parent_dir3=%parent_dir2%\.."
cd %parent_dir3%
set "parent_dir3=%cd%"


set WORKSPACE=%parent_dir%
set PARENT_WORKSPACE=%parent_dir2%


svn revert -R  %PARENT_WORKSPACE%\configTools
svn update %PARENT_WORKSPACE%\configTools

::svn revert -R  %PARENT_WORKSPACE%\design
::svn update %PARENT_WORKSPACE%\design

::svn revert -R  %PARENT_WORKSPACE%\config
::svn update %PARENT_WORKSPACE%\config

svn revert -R  %PARENT_WORKSPACE%\trunk\Dev
svn update %PARENT_WORKSPACE%\trunk\Dev


pause