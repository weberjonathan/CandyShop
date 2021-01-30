# Candy Shop
![CandyShop Example](/docs/example.jpg)

_Candy Shop_ (formely _Choco AutoUpdate_) is a minimal UI for managing installed _[Chocolatey](https://chocolatey.org/)_ packages. It may also check for outdated packages in the background, notify the user and allow the installation of new versions by the push of a button. The application also provides an overview of installed packages.

* Requires `choco` to be in PATH.

__[Visit the gallery.](/docs/gallery.md)__

## Features
* Start the application and see outdated and installed packages.
* Upgrade all or a selected number of outdated packages.
* Schedule the launch using the Windows Task Scheduler and specify the `--background` (or `-b`) option to check for outdated packages in the background on system start.

## TODO
* Allow user to specify Chocolatey binary
* Offer optional functionality to automatically create entry in Windows Task scheduler
* Search and filter options on Installed tab
* Search for and install new software

## Issues
* Once the notification is moved to the Windows 10 Action center it can no longer be clicked. Click the tray icon instead.
