# Candy Shop
![CandyShop Example](/docs/example.jpg)
<img src="/docs/upgrade.jpg" alt="upgrade view" height="233px"/>

_Candy Shop_ offers a simple user interface to upgrade any number
of packages installed through either
_[WinGet](https://github.com/microsoft/winget-cli)_ or 
_[Chocolatey](https://chocolatey.org/)_. The upgrade process
can be launched directly from the notification or for a selection
of packages from the application window. The upgrades are performed
in a visible terminal, allowing the user to inspect the package
manager's output directly.

__[Visit the gallery.](/docs/gallery.md)__

## Features
* Upgrade outdated packages
* Notifies about outdated packages on system start, if desired.
* Pin or unpin packages
* Browse installed packages
* Automatically remove shortcuts from your desktop that are created during the Chocolatey upgrade process

## Prerequisites
- Windows 10, version 1809 or later
- [.NET 8 runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- [gsudo](https://github.com/gerardog/gsudo) (optional)
- [Chocolatey](https://chocolatey.org/) or [WinGet](https://github.com/microsoft/winget-cli)

```
winget install Microsoft.DotNet.DesktopRuntime.8
winget install gerardog.gsudo
```

In WinGet mode, Candy Shop will attempt to validate the
winget output with locale dependent strings. Currently, this
is only supported for English and German.
[Learn more.](/docs/locales.md)

## Install
- Make sure the prerequisites are met
- Download and run the installer from the [release page](https://github.com/weberjonathan/CandyShop/releases)
- Read the [settings](#settings) section

If you're running WinGet and have not used the tool before, you
may need to accept licensing agreements in an interactive console.
To test for this, simply run `winget list` in the terminal and follow
the instructions. If you see a list of your installed packages, you're
good to go.

## Security

Candy Shop can be configured to start a gsudo cache session to
prevent repeated UAC popups for upgrade installation in WinGet
mode. The session will be started at the beginning of the upgrades and terminated
once they are complete, and no changes are made to the gsudo configuration.

Note that this does not apply to Chocolatey upgrades, because
(unlike WinGet) Chocolatey allows several upgrades to be started using a single command, thus eliminating the need for repeated popups.

No other batch operations are performed that require administrator
privileges.
[Learn more about the security implications of the gsudo credentials cache.](https://github.com/gerardog/gsudo?tab=readme-ov-file#credentials-cache)

## Settings

On first launch, you will be prompted to choose your package manager
and your preferred privileges for upgrading any packages. To make
changes to your selections simply modify the configuration file
or delete it entirely, resulting in the first launch popup being
shown again. Most settings can also be changed in the UI.

By default, Candy Shop is configured to use `choco` and `winget`
from PATH. __It is recommended to set these explicitly
in the configuration file.__

Configuration and log files are placed in `%localappdata%\CandyShop`. The configuration file `CandyShop.config` contains the following properties with their respective defaults:

```json
{
  "ChocolateyBinary": "choco",
  "ChocolateyLogs": "C:/ProgramData/chocolatey/logs",
  "WingetBinary": "winget",
  "AllowGsudoCache": false,
  "WingetMode": true,
  "CleanShortcuts": false,
  "ElevateOnDemand": true,
  "SupressAdminWarning": false,
  "SupressLocaleLogWarning": false,
  "CloseAfterUpgrade": false,
  "ValidExitCodes": [ 0, 1641, 3010, 350, 1604 ]
}
```

- `WingetMode`: If true, the selected package manager is WinGet, else it is Chocolatey.
- `ChocolateyBinary`, `WingetBinary`: Path to the respective binaries. These should be overwritten with explicit locations.
- `ElevateOnDemand`: When activated, `gsudo` is used to attempt to elevate the package manager process.
- `AllowGsudoCache`: Whether to start a gsudo cache session for batch operations that require elevated permissions. (This currently only applies to the WinGet upgrade process.)
- `CloseAfterUpgrade`: If this is `true`, Candy Shop will terminate after upgrading packages, as long as the operation was successful. Otherwise, the main window will be shown again.
- `ValidExitCodes`: Depending on the configuration of Chocolatey and the package in question, some non-zero exit codes when upgrading packages should be considered valid. This setting provides sensible defaults and allows further customization. (Chocolatey only.)
- `CleanShortcuts`: If this setting is enabled, Candy Shops watches the Desktop for new shortcuts, that are created during an active upgrade process, and attempts to delete them.
- `SupressLocaleLogWarning`: Suppresses warnings about unsupported locales in the log files. [More about languages.](/docs/locales.md)

## Command line arguments
`--silent`, `-s` is used to launch silently and notify when outdated packages are available.

`--debug` is used to print debug level log messages to your log file.

## Build Instructions

- [dotnet publish](https://docs.microsoft.com/en-us/dotnet/core/tools/dotnet-publish)

## Changelog

### 0.11.0

- added full winget support
- added wizard for first start
- various UI improvements
- added upgrade all button notification

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

## Known issues

- If credentials are cached using gsudo and the upgrade process takes longer than the allowed cache duration, no attempt is made to start another cached session, resulting in repeated UAC popups.

## Roadmap

- Create CandyShop winget package

__v1.0__:

- For the first release, a settings page and a re-organization of the configuration file are the only things left.

__Future versions__

- Migrate to WinUI