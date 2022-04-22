Write-Host 'Welcome to the CandyShop build script'
Write-Host

# resolve project dir
$PROJECT_DIR = "";
$CURRENT_DIR = Split-Path -Path $pwd -Leaf;
if ($CURRENT_DIR -eq "scripts")
{
    $PROJECT_DIR = Split-Path -Path $pwd -Parent -Resolve;
}
elseif ($CURRENT_DIR -eq "CandyShop")
{
    $PROJECT_DIR = $pwd;
}

if ((Test-Path "$PROJECT_DIR/CandyShop/CandyShop.csproj") -eq $false)
{
    Write-Host -BackgroundColor Black -ForegroundColor Red "ERROR: Could not find project file. Make sure to execute this script from project root or the scripts folder."
    return;
}

# check if dotnet is installed
$dotnetCheck = (Get-Command "dotnet" -ErrorAction SilentlyContinue)
if ($dotnetCheck -eq $null) {
    Write-Host "ERROR: 'dotnet' not found in path. Aborting build.";
    return;
}

$dotnetCheck = dotnet --list-sdks;
if ($dotnetCheck -eq $null -or $dotnetCheck -eq "") {
    Write-Host "ERROR: Please install .NET Core SDK version 3.1 or higher. Aborting build.";
    return;
}

# get self contained option
Write-Host "Should the application be packaged with the .NET Core runtime?"
Write-Host "Please enter a number or press Enter to accept the default."
Write-Host "[1] No, the application should rely on the installed .NET Core Runtime. (Default)"
Write-Host "[2] Yes, the application should work as a standalone app."
$inclRuntime = $null
while ($inclRuntime -ne "13" -and $inclRuntime -ne "49" -and $inclRuntime -ne "50") {
    $inclRuntime = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode
    # VirtualKeyCodes: 13 => Enter, 49 => 1, 50 => 2
}

if ($inclRuntime -eq "50")
{
    $option_NoSelfContained = "";
    Write-Host "Your selection: 2"
}
else
{
    $option_NoSelfContained = "--no-self-contained";
    Write-Host "Your selection: 1 (Default)"
}
Write-Host

# platform option
# out dir

# admin / no admin
Write-Host "Always require admin privileges on launch?"
Write-Host "[1] Yes. (Default)"
Write-Host "[2] No."
$option_NoAdmin = $null
while ($option_NoAdmin -ne "13" -and $option_NoAdmin -ne "49" -and $option_NoAdmin -ne "50") {
    $option_NoAdmin = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown').VirtualKeyCode
    # VirtualKeyCodes: 13 => Enter, 49 => 1, 50 => 2
}

if ($option_NoAdmin -eq "50")
{
    Write-Host "Your selection: 2"
    Move-Item "$PROJECT_DIR/CandyShop/app.manifest" "$PROJECT_DIR/CandyShop/app.manifest.admin"
    Move-Item "$PROJECT_DIR/CandyShop/app.manifest.noadmin" "$PROJECT_DIR/CandyShop/app.manifest"
    $option_NoAdmin = $true;
}
else
{
    Write-Host "Your selection: 1 (Default)"
    $option_NoAdmin = $false;
}

# clean old files
if (Test-Path "$PROJECT_DIR/build")
{
    Write-Host -NoNewline "Cleaning build directory ... ";
    Remove-Item -Recurse "$PROJECT_DIR/build";
    Write-Host "Done."
}

Write-Host
Write-Host -ForegroundColor Gray -BackgroundColor Black "$ dotnet publish $PROJECT_DIR/CandyShop/CandyShop.csproj -c Release -f netcoreapp3.1 -p:PublishReadyToRun=true -p:PublishSingleFile=true $option_NoSelfContained -r win-x64 -o $PROJECT_DIR/build"
dotnet publish $PROJECT_DIR/CandyShop/CandyShop.csproj -c Release -f netcoreapp3.1 -p:PublishReadyToRun=true -p:PublishSingleFile=true $option_NoSelfContained -r win-x64 -o $PROJECT_DIR/build
Write-Host

# clean up
Write-Host -NoNewline 'Cleaning up ... '
if (Test-Path "$PROJECT_DIR/build/CandyShop.pdb")
{
    Remove-Item "$PROJECT_DIR/build/CandyShop.pdb"
}
if ($option_NoAdmin) {
    Move-Item "$PROJECT_DIR/CandyShop/app.manifest" "$PROJECT_DIR/CandyShop/app.manifest.noadmin"
    Move-Item "$PROJECT_DIR/CandyShop/app.manifest.admin" "$PROJECT_DIR/CandyShop/app.manifest"
}
Write-Host "Done."

$execFilepath = Resolve-Path "$PROJECT_DIR/build/CandyShop.exe";
Write-Host -ForegroundColor Green -BackgroundColor Black "Build successful! The application can be found at '$execFilepath'"

Write-Host ''
Write-Host -NoNewLine 'Press any key to continue...';
$key = $Host.UI.RawUI.ReadKey('NoEcho,IncludeKeyDown');