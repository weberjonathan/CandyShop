# Candy Shop
![CandyShop Example](/docs/example.jpg)
<img src="/docs/upgrade.jpg" alt="upgrade view" height="184px"/>

_Candy Shop_ offers a lightweight user interface to manage installed _[Chocolatey](https://chocolatey.org/)_ packages and can inform the user of available upgrades.

__[Visit the gallery.](/docs/gallery.md)__

* Tested on Windows 10
* Requires .NET Core 3.1
* Requires `choco` to be in PATH.
* Requires administrator privileges. Refer to build instructions if this is not desired.

## Features
* Displays outdated packages on launch and allows the user to upgrade any number of them.
* Displays installed packages in a separate tab and loads package details when needed.
* Can silently check for outdated packages on login and inform user of available upgrades. (Launch silently at any time by using the `--background` or `-b` option.)

## Settings
Configuration and log files are placed in `%localappdata%\CandyShop`. The configuration file `CandyShop.config` contains the following properties with their respective defaults:
```json
{
  "ChocolateyLogs": "C:\\ProgramData\\chocolatey\\logs",
  "CleanShortcuts": false
}
```

## Build Instructions
A build script is available at `.\CandyShop\scripts\build.ps1`.
1. Make sure the .NET Core SDK 3.1 or higher is installed on your system or download it (https://dotnet.microsoft.com/download/dotnet/3.1) and launch a powershell console.
1. Clone the repo: `git clone https://github.com/weberjonathan/CandyShop.git`
2. Navigate to project root or scripts folder: `cd .\CandyShop\`
3. Execute build script: `.\build.ps1`
4. Locate executable in `\CandyShop\build\`

The build script is interactive and asks, whether the runtime should be included in the build and whether launching CandyShop requires administrator privileges, or not. Defaults are provided.

See https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish for further build instructions.

(Note, that by default CandyShop requires adminstrator privileges, which depending on your setup may not always be necessary for your Chocolatey installation. To build CandyShop without this requirement, simply switch the [`app.manifest`](CandyShop/app.manifest)-file with [`app.manifest.noadmin`](CandyShop/app.manifest.admin) in your repository.)

## Notes
* When the "Clean shortcuts" option is selected, any desktop shortcuts that are created between the start and the end of the upgrade process will be deleted. A record of any such operation is logged.
* Open the Chocolatey log folder by clicking the respective menu item. If this differs from the default installation directory, please change this value in the CandyShop settings file.

### Issues
* Once the notification is moved to the Windows 10 Action center it can no longer be clicked. Click the tray icon instead.

### TODOs
* Allow user to specify Chocolatey binary
* Search for and install new software
* Evaluate modern Win10 notifications and WinUI 3
* Evaluate winget