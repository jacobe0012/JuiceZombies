@echo off
setlocal

rem 切换到UTF-8编码，以正确显示中文
chcp 65001 > nul

echo =======================================================
echo 开始 EF Core 数据库重置流程
echo =======================================================
echo.

rem =======================================================
rem 第1步：切换到上一级目录
rem =======================================================
echo 步骤 1/5: 正在切换到上一级目录...
cd ..
echo 当前目录已切换到: %cd%
echo.
set /p DUMMY="请按任意键继续..."
echo.

rem =======================================================
rem 第2步：删除本地所有迁移文件并重建文件夹
rem =======================================================
echo 步骤 2/5: 正在删除 Migrations 文件夹下的所有内容...
if exist "Migrations" (
    rmdir /s /q "Migrations"
    echo Migrations 文件夹已删除。
) else (
    echo Migrations 文件夹不存在。
)
mkdir "Migrations"
echo Migrations 文件夹已重新创建。
echo.
set /p DUMMY="请按任意键继续..."
echo.

rem =======================================================
rem 第3步：删除数据库
rem =======================================================
echo 步骤 3/5: 正在删除数据库，此操作将永久删除所有数据...
echo y|dotnet ef database drop
if %errorlevel% neq 0 (
    echo.
    echo 错误：数据库删除命令失败。请查看上面的错误信息。
    goto :error_end
)
echo 数据库已成功删除。
echo.
set /p DUMMY="请按任意键继续..."
echo.

rem =======================================================
rem 第4步：重新创建初始迁移
rem =======================================================
echo 步骤 4/5: 正在创建新的初始迁移...
dotnet ef migrations add InitialCreate
if %errorlevel% neq 0 (
    echo.
    echo 错误：创建迁移命令失败。请查看上面的错误信息。
    goto :error_end
)
echo 新的初始迁移已创建。
echo.
set /p DUMMY="请按任意键继续..."
echo.

rem =======================================================
rem 第5步：更新数据库，并应用新的迁移
rem =======================================================
echo 步骤 5/5: 正在更新数据库并应用迁移...
dotnet ef database update
if %errorlevel% neq 0 (
    echo.
    echo 错误：更新数据库命令失败。请查看上面的错误信息。
    goto :error_end
)
echo 数据库已成功更新并初始化。
echo.

goto :end

:error_end
echo.
echo =======================================================
echo ❌ 脚本执行失败。
echo =======================================================
goto :final_end

:end
echo.
echo =======================================================
echo ✅ 脚本已成功完成。
echo =======================================================

:final_end
echo.
echo 按任意键退出...
pause > nul
endlocal