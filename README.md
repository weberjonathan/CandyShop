# chocoHelpers
_ChocoAutostart_ checks for outdated choco updates and prompts once to upgrade all (using integrated upgrade tool with elevated privileges).

Use `ChocoAutostart.exe -d` to display debug information.

Targets .NET Core 3.1. Written for Chocolatey v0.10.15.

## ChocoAutostart Versions
### 1.1.0
* Automatically removes shortcuts created during package upgrade.

## ChocoUpgrade Versions
### 1.1.0
* Use `ChocoUpgrade.exe -c` or `--cleanup` to remove shortcuts created during the installation of upgrades.

## Help
* `%appdata%\Microsoft\Windows\Start Menu\Programs\Startup`
* `shell:startup`

## TODO
* Migrate initial UI to Windows 10 notification center
