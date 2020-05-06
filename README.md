# chocoHelpers
_ChocoAutostart_ checks for outdated choco updates and prompts once to upgrade all. Packages _ChocoUpgrade_ and launches it on demand (and with elevated privileges).

Use `ChocoAutostart.exe -d` to display debug information.
Use `ChocoUpgrade.exe -c` or `--cleanup` to automatically remove newly created shortcuts on the desktop. The default release package uses this option.

Targets .NET Core 3.1. Written for Chocolatey v0.10.15.

## Other
* `%appdata%\Microsoft\Windows\Start Menu\Programs\Startup`
* `shell:startup`

## TODO
* Migrate initial UI to Windows 10 notification center
