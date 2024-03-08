# Candy Shop
![CandyShop Example](/docs/example.jpg)
<img src="/docs/upgrade.jpg" alt="upgrade view" height="233px"/>

_Candy Shop_ offers a simple user interface to manage installed _[Chocolatey](https://chocolatey.org/)_ packages and notifies about outdated packages, if desired.

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

## Install
- Make sure the prerequisites are met
- Download and extract [latest release](https://github.com/weberjonathan/CandyShop/releases)
- Supported languages: English, German. [More about languages.](/docs/locales.md)

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
  "SupressLocaleLogWarning": false,
  "CloseAfterUpgrade": false,
  "ValidExitCodes": [ 0, 1641, 3010, 350, 1604 ]
}
```

- `ElevateOnDemand`: When activated, `gsudo` is used to attempt to elevate the Chocolatey upgrade process. Note that running Candy Shop with administrator privileges renders this setting obsolete.
- `CloseAfterUpgrade`: If this is `true`, Candy Shop will terminate after upgrading packages, as long as the operation was successful. Otherwise, the main window will be shown again.
- `ValidExitCodes`: Depending on the configuration of Chocolatey and the package in question, some non-zero exit codes when upgrading packages should be considered valid. This setting provides sensible defaults and allows further customization.
- `CleanShortcuts`: If this setting is enabled, Candy Shops watches the Desktop for new shortcuts, that are created during an active upgrade process, and attempts to delete them.
- `WingetMode`: Experimental and non-functional.
- `SupressLocaleLogWarning`: Suppresses warnings about unsupported locales in the log files. [More about languages.](/docs/locales.md)

## Command line arguments
`--background`, `-b` is used to launch silently and notify when outdated packages are available.

`--debug` is used to print debug level log messages to your log file.

## Build Instructions

- [dotnet publish](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)

## Changelog

### 0.10.0

- various UI improvements
- added refresh functionality
- returning to the main window after the upgrade no longer shows outdated states

### 0.9.0

- Introduced property and UI element to customize shutdown behavior
- The application no longer terminates after failed upgrades and instead redirects to the main window.
- Fixed an error, where pinning was impossible because of missing admin privileges
- Fixed an error, where installed packages were not loaded if Candy Shop was launched in the background

### 0.8.0

- Replaced old notifications with modern, interactive notifications using Windows App SDK
- 'start with system' now uses a shortcut in the startup folder instead of Windows task scheduler
- The legacy task can be removed through the prompt at program launch

### 0.6.0

- Moved to latest .NET LTS release (.NET 8)
- This changes the requirements of the runtime version
- Windows 10 is now considered the latest supported version

### 0.5.0

- Candy Shop no longer requires admin privileges by default, instead it prompts for elevated rights using `gsudo`

## Similar projects

- [Chocolatey GUI](https://github.com/chocolatey/ChocolateyGUI)
- [WingetUI](https://github.com/marticliment/WingetUI)

## TODO
* Search for and install new software
* Add support for winget
