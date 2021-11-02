# Candy Shop
![CandyShop Example](/docs/example.jpg)

_Candy Shop_ is a minimal UI for managing installed _[Chocolatey](https://chocolatey.org/)_ packages. It may also check for outdated packages in the background, notify the user and allows upgrading packages through the push of a button. The application also provides an overview of installed packages.

* Requires `choco` to be in PATH.
* Default build requires administrator privileges. If this is not desired please swap the [`app.manifest`](CandyShop/app.manifest)-file with [`app.manifest.noadmin`](CandyShop/app.manifest.admin) and build the software using the instructions below.

__[Visit the gallery.](/docs/gallery.md)__

## Features
* Start the application and see outdated and installed packages. Also shows detailed information for installed packages.
* Upgrade all or a selected number of outdated packages.
* Launch in the background on system start and be notified of outdated packages. (Launch in the background at any time using the `--background` or `-b` option.)

## Build Instructions
1. Make sure the .NET Core SDK 3.1 or higher is installed on your system (`dotnet --version`) or download it (https://dotnet.microsoft.com/download/dotnet-core/3.1).
1. `git clone https://github.com/weberjonathan/CandyShop.git -b release-0.2`
2. `cd ./CandyShop/CandyShop/`
3. Run `dotnet publish` with appropriate options. See example below or visit https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish for more information.

__Example__
`dotnet publish CandyShop.csproj -c Release -f netcoreapp3.1 -p:PublishReadyToRun=true -p:PublishSingleFile=true --no-self-contained -r win-x64 -o ./bin`

Unlike the build in the release section, this build depends on the dotnet core runtime environment and the respective executable is significantly reduced in size. Omit `--no-self-contained` to create a build that contains the runtime environment. (For this, you may use `-p:PublishTrimmed=true` as well.)

Change `win-x64` to `win-x86` to build an 32-bit application. See https://docs.microsoft.com/en-us/dotnet/core/rid-catalog#windows-rids for more values.

In this example, the executable can be found in `./bin` and moved whereever.

## TODO
* Allow user to specify Chocolatey binary
* Search for and install new software

## Issues
* Once the notification is moved to the Windows 10 Action center it can no longer be clicked. Click the tray icon instead.
