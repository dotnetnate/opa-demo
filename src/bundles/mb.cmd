@echo off
setlocal

REM Check if the argument is provided
if "%1"=="" (
    echo Usage: build-bundle.bat [foldername]
    exit /b 1
)

REM Set the folder name and output file name
set "FOLDERNAME=%1"
set "OUTPUTFILE=%FOLDERNAME%\%FOLDERNAME%.tar.gz"

REM Run eopa build on the specified folder
eopa build -b %FOLDERNAME% -o %OUTPUTFILE%

REM Check if the build was successful
if errorlevel 1 (
    echo Failed to build the bundle.
    exit /b 1
)

REM Copy the output file to the root bundles directory
copy %OUTPUTFILE% ..\..\bundles\

REM Check if the copy was successful
if errorlevel 1 (
    echo Failed to copy the bundle to the root bundles directory.
    exit /b 1
)

echo Bundle %OUTPUTFILE% successfully built and copied to the root bundles directory.

endlocal