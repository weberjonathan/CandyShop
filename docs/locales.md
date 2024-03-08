# Languages

Localized language strings are used to validate the output of Winget. If the Windows installation uses a non-supported language, the validation will fail and respective log messages are printed, unless the setting `SupressLocaleLogWarning` is `true`. Note that for non-supported languages the output is still expected to parse correctly.

List of supported Languages:
- English
- German

## Contributing language support

Two strings are used to validate the Winget output. In the table below, examples are given for the required string. If you are using a non-supported language, consider running the examples and opening a GitHub issue with the strings on your system, as well as the your language, so support can be added.

| String (EN) | Description | Example |
          --- | ---         | ---     |
| Found       | Represents the first word printed by package searches.     | `winget show Microsoft.AppInstaller` |
| Name        | Represents the name of the first column of package lists.  | `winget list` |
| "Es sind keine Pins konfiguriert." | Printed when the pin list is empty. | `winget pin list` |