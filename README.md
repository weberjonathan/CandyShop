# Candy Shop
![CandyShop Example](/docs/example.jpg)
<img src="/docs/upgrade.jpg" alt="upgrade view" height="184px"/>

_Candy Shop_ offers a lightweight user interface to manage installed _[Chocolatey](https://chocolatey.org/)_ packages, including alerts for outdated packages.

__[Visit the gallery.](/docs/gallery.md)__

## Features
* Show outdated packages and update some or all of them with the click of a button
* Browse installed packages in a separate tab
* Launch minimized and check for outdated packages on login
* Pin or unpin packages from the upgrade-tab
* Automatically remove shortcuts from your desktop that are created during the Chocolatey upgrade process

## Prerequisites
- Windows 10, version 1809 or later
- [.NET 8 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [Chocolatey](https://chocolatey.org/) (versions 1.x and 2.x)
- [gsudo](https://github.com/gerardog/gsudo) (recommended)

Note that gsudo in an optional dependency, but strongly recommended in order for the Chocolatey upgrade process to run with elevated privileges without Candy Shop being elevated itself. It may be installed by running `choco install gsudo`.

The current build was tested on Windows 10 Version 22H2 and with Chocolatey version 2.2.2. 

## Settings
Configuration and log files are placed in `%localappdata%\CandyShop`. The configuration file `CandyShop.config` contains the following properties with their respective defaults:
```json
{
  "ChocolateyBinary": "C:/ProgramData/chocolatey/bin/choco.exe",
  "ChocolateyLogs": "C:/ProgramData/chocolatey/logs",
  "WingetMode": false,
  "CleanShortcuts": false,
  "ElevateOnDemand": true,
  "SupressAdminWarning": false,
  "ValidExitCodes": [ 0, 1641, 3010, 350, 1604 ]
}
```

- `ElevateOnDemand`: When activated, `gsudo` is used to attempt to elevate the Chocolatey upgrade process. Note that running Candy Shop with administrator privileges renders this setting obsolete.
- `ValidExitCodes`: Depending on the configuration of Chocolatey and the package in question, some non-zero exit codes when upgrading packages should be considered valid. This setting provides sensible defaults and allows further customization.
- `CleanShortcuts`: If this setting is enabled, Candy Shops watches the Desktop for new shortcuts, that are created during an active upgrade process, and attempts to delete them.
- `WingetMode`: Experimental and non-functional.

## Command line arguments
`--background`, `-b` is used to launch silently and notify when outdated packages are available.

`--debug` is used to print debug level log messages to your log file.

## Build Instructions

- [dotnet publish](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)

## Changelog

### 0.7.0

- Replaced old notifications with modern, interactive notifications
- Requires Windows App SDK, which comes preinstalled with Windows 10 version 1809

### 0.6.0

- Moved to latest .NET LTS release (.NET 8)
- This changes the requirements of the runtime version
- Windows 10 is now considered the latest supported version

### 0.5.0

- Candy Shop no longer requires admin privileges by default, instead it prompts for elevated rights using `gsudo`

## TODO
* Search for and install new software
* Add support for winget
