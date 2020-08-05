# Choco AutoUpdate
![Choco AutoUpdate](/docs/example.jpg)

_Choco AutoUpdate_ checks for outdated _[Chocolatey](https://chocolatey.org/)_ packages in the background and prompts the user if any are found. The packages in question will be listed and upgraded after confirmation by the user.

In essence, this serves as a minimal UI for Chocolatey for upgrading purposes only.

* Requires `choco` to be in PATH.
* The launch option `--hide-admin-warn` has been deprecated, since new UI components make it obsolete.

## Help
* Windows Task Scheduler
* `%appdata%\Microsoft\Windows\Start Menu\Programs\Startup`
* `shell:startup`
