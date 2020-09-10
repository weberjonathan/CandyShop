# Candy Shop
![Choco AutoUpdate](/docs/example.jpg)

_Candy Shop_ (formely _Choco AutoUpdate_) is a minimal UI for managing installed _[Chocolatey](https://chocolatey.org/)_ packages. It checks for outdated packages in the background, notifies the user and installs them by the push of a button. The application also provides an overview of installed packages.

* Requires `choco` to be in PATH.

## Features
* Start the application and see outdated and installed packages.
* Schedule the launch using the Windows Task Scheduler and specify the `--silent` (or `-s`) option to check for outdated packages in the background on system start.

## Issues
* Once the notification is moved to the Windows 10 Action center it can no longer be clicked. Click the tray icon instead
