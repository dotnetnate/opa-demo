@echo off
setlocal

REM Iterate over each subfolder in the current directory
for /d %%d in (*) do (
    REM Set the folder name and output file name
    set "FOLDERNAME=%%d"
    set "OUTPUTFILE=%%d\%%d.tar.gz"

    REM Run eopa build on the specified folder
    eopa build -b %%d -o %%d.tar.gz

    REM Check if the build was successful
    if errorlevel 1 (
        echo Failed to build the bundle for %%d.
        exit /b 1
    )

    REM Copy the output file to the root bundles directory
    copy %%d.tar.gz ..\..\bundles\

    REM Check if the copy was successful
    if errorlevel 1 (
        echo Failed to copy the bundle for %%d to the root bundles directory.
        exit /b 1
    )

    echo Bundle %%d.tar.gz successfully built and copied to the root bundles directory.
)

endlocal