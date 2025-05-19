using CandyShop.Settings;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CandyShop.PackageCore
{
    internal class PackageManagerFactory
    {
        public static AbstractPackageManager Chocolatey(SettingsDefinition settings)
        {
            return new ChocoManager(
                2, // TODO
                settings.Chocolatey.ValidExitCodes,
                settings.Chocolatey.Filepath);
        }

        public static AbstractPackageManager Winget(SettingsDefinition settings)
        {
            return new WingetManager(settings.Winget.Filepath);
        }

        public static AbstractPackageManager Active(SettingsDefinition settings)
        {
            if (settings.Winget.Enabled)
                return Winget(settings);
            else if (settings.Chocolatey.Enabled)
                return Chocolatey(settings);
            else
                throw new ArgumentException();
        }
    }

    [Flags]
    internal enum PackageManagerFilters
    {
        HideSuffix,
        RequireSource
    }

    internal abstract class AbstractPackageManager(string binary)
    {
        public string Binary { get; private set; } = binary;
        public abstract bool SupportsPinningAsUser { get; }
        public abstract bool SupportsFetchingOutdated { get; }
        public abstract bool RequiresNameResolution { get; }
        public abstract string Name { get; }
        public abstract PackageManagerFilters SupportedFilters { get; }

        /// <summary>
        /// Validates the package manager and returns the validated version.
        /// </summary>
        /// <returns>Returns a validated version in the form major.minor.build or throws exceptions</returns>
        /// <exception cref="PackageManagerException"></exception>
        public abstract string ValidateExec();

        /// <exception cref="PackageManagerException"></exception>
        /// <exception cref="CandyShopException"></exception>
        public abstract void Upgrade(List<GenericPackage> packages, bool useGsudo = false, bool enableGsudoCache = false);

        /// <summary>
        /// Tries to resolve incomplete package names and returns a list of
        /// successful resolutions and the original packages in cases, where
        /// package name resolution failed.
        /// </summary>
        /// <param name="unresolved"></param>
        /// <returns></returns>
        public abstract Task<GenericPackage[]> ResolveAbbreviatedNamesAsync(List<GenericPackage> unresolved);

        /// <exception cref="PackageManagerException"></exception>
        public abstract void OpenLogFolder();

        /// <exception cref="PackageManagerException"></exception>
        public async Task<GenericPackage[]> FetchInstalledAsync()
        {
            return await Task.Run(FetchInstalled);
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task<GenericPackage[]> FetchOutdatedAsync()
        {
            return await Task.Run(FetchOutdated);
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task<GenericPackage[]> FetchPinListAsync()
        {
            return await Task.Run(FetchPinList);
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task<string> FetchInfoAsync(GenericPackage package)
        {
            return await Task.Run(() => FetchInfo(package));
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task PinAsync(GenericPackage package, bool useGsudo = false)
        {
            await Task.Run(() => Pin(package, useGsudo));
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task UnpinAsync(GenericPackage package, bool useGsudo = false)
        {
            await Task.Run(() => Unpin(package, useGsudo));
        }

        /// <exception cref="PackageManagerException"></exception>
        protected abstract GenericPackage[] FetchInstalled();

        /// <exception cref="PackageManagerException"></exception>
        protected abstract GenericPackage[] FetchOutdated();

        /// <exception cref="PackageManagerException"></exception>
        protected abstract GenericPackage[] FetchPinList();

        /// <exception cref="PackageManagerException"></exception>
        protected abstract string FetchInfo(GenericPackage package);

        /// <exception cref="PackageManagerException"></exception>
        protected abstract void Pin(GenericPackage package, bool useGsudo = false);

        /// <exception cref="PackageManagerException"></exception>
        protected abstract void Unpin(GenericPackage package, bool useGsudo = false);

        /// <exception cref="CandyShopException"></exception>
        protected void InitGsudoCache()
        {
            Process p = new()
            {
                StartInfo = new("gsudo", $"cache on -p {Environment.ProcessId}")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            try
            {
                p.Start();
                p.WaitForExit();
            }
            catch (Exception e)
            {
                throw new CandyShopException($"Failed to create gsudo credentials cache session (gsudo returned {p.ExitCode}). To inspect active cache sessions run \"gsudo status\" in the terminal.", e);
            }

            if (p.ExitCode != 0)
                throw new CandyShopException($"Failed to create gsudo credentials cache session (gsudo returned {p.ExitCode}). To inspect active cache sessions run \"gsudo status\" in the terminal.");
        }

        /// <exception cref="CandyShopException"></exception>
        protected void DisableGsudoCache()
        {
            Process p = new()
            {
                StartInfo = new("gsudo", $"cache off -p {Environment.ProcessId}")
                {
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            try
            {
                p.Start();
                p.WaitForExit();
            }
            catch (Exception e)
            {
                throw new CandyShopException($"Failed to stop gsudo credentials cache session (gsudo returned {p.ExitCode}). To mitigate any security risk, please run 'gsudo -k' in a terminal to end all gsudo cache sessions, or restart your system.", e);
            }

            if (p.ExitCode != 0)
                throw new CandyShopException($"Failed to stop gsudo credentials cache session (gsudo returned {p.ExitCode}). To mitigate any security risk, please run 'gsudo -k' in a terminal to end all gsudo cache sessions, or restart your system.");
        }

        protected virtual PackageManagerProcess BuildProcess(string args, bool useGsudo = false)
        {
            return useGsudo ? new("gsudo", $"{Binary} {args}") : new(Binary, args);
        }
    }
}
