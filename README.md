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
* Automatically remove unwanted shortcuts from your desktop that are created during the Chocolatey upgrade process. (See notes section.)
* Open the Candy Shop configuration directory `%localappdata%\CandyShop` through the menu and edit your settings.

## Settings
Configuration and log files are placed in `%localappdata%\CandyShop`. The configuration file `CandyShop.config` contains the following properties with their respective defaults:
```json
{
  "ChocolateyLogs": "C:\\ProgramData\\chocolatey\\logs",
  "CleanShortcuts": true,
  "ValidExitCodes": [ 0, 1641, 3010, 350, 1604 ]
}
```

Chocolatey considers some non-zero exit codes for the upgrade process valid. This can depend on the installed packages, which is what the property is for.

Regardless of this value, the full output of the Chocolatey upgrade process is shown when upgrading packages through Candy Shop. This includes exit codes.

## Command line arguments
`--background`, `-b` is used to launch silently and notify when outdated packages are available.

`--debug` is used to print debug level log messages to your log file.

## Build Instructions
A build script is available at `.\CandyShop\scripts\build.ps1`.

1. Make sure the .NET Core SDK 3.1 or higher is installed on your system or find and download it [here](https://dotnet.microsoft.com/download/dotnet/3.1). Launch a powershell console.
1. Clone the repo: `git clone https://github.com/weberjonathan/CandyShop.git`
2. Navigate to project root or scripts folder: `cd .\CandyShop\`
3. Execute build script: `.\build.ps1`
4. Locate executable in `\CandyShop\build\`

The build script is interactive and asks, whether the runtime should be included in the build and whether launching CandyShop should always require administrator privileges, or not. Defaults are provided.

You may need to adjust your PowerShell execution policies in order to run the build script. Learn more about this topic [here](https://docs.microsoft.com/en-us/powershell/module/microsoft.powershell.core/about/about_execution_policies?view=powershell-7.2).

Learn about the [dotnet publish](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish) command for further build instructions.

(Note, that by default CandyShop requires adminstrator privileges, which depending on your setup may not always be necessary for your Chocolatey installation. To build CandyShop without this requirement, simply swap the [`app.manifest`](CandyShop/app.manifest)-file with [`app.manifest.noadmin`](CandyShop/app.manifest.admin).)

## Notes
* When the "Clean shortcuts" option is selected, any desktop shortcuts that are created between the start and the end of the upgrade process will be deleted. A record of any such operation is logged.
* Open the Chocolatey log folder by clicking the respective menu item. If this differs from the default installation directory, please change this value in the CandyShop settings file.

### Issues
* Once the notification is moved to the Windows 10 Action center it can no longer be clicked. Click the tray icon instead.

### TODOs
* Search for and install new software
* Evaluate modern Win10 notifications and WinUI 3
* Target latest LTS .NET Core Runtime
* Add support for winget
* Support marking packages as pinned
* Allow user to specify Chocolatey binary