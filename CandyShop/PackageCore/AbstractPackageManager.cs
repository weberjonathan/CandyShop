using CandyShop.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CandyShop.PackageCore
{
    internal class PackageManagerFactory
    {
        public static AbstractPackageManager Create(PackageManagerDefinition pm, bool useGsudo = false, bool useCredentialsStore = false)
        {
            if ("Winget".Equals(pm.Name))
            {
                return new WingetManager(pm.Filepath, useGsudo, useCredentialsStore);
                // TODO supress log warning thingy
            }
            else if ("Chocolatey".Equals(pm.Name))
            {
                return new ChocoManager(2, pm.ValidExitCodes, pm.Filepath, useGsudo, useCredentialsStore);
                // TODO version
            }
            else
            {
                throw new ArgumentException("Unknown active package manager.");
            }
        }

        public static AbstractPackageManager Create(PackageManagerDefinition pm, SettingsDefinition settings)
        {
            return Create(pm, settings.ElevateOnDemand, settings.Gsudo.EnableCredentialsStore);
        }

        public static AbstractPackageManager Create(SettingsDefinition settings)
        {
            return Create(settings.PackageManagers[settings.ActivePackageManager], settings);
        }
    }
    
    internal abstract class AbstractPackageManager(string binary, bool useGsudo, bool useCredentialsStore)
    {
        public bool UseGsudo { get; } = useGsudo;
        public abstract bool SupportsPinningAsUser { get; }
        public abstract bool SupportsFetchingOutdated { get; }
        public abstract bool RequiresNameResolution { get; }

        protected string Binary { get; private set; } = binary;
        protected bool AllowGsudoCache { get; } = useCredentialsStore;

        /// <summary>
        /// Validates the package manager and returns the validated version.
        /// </summary>
        /// <returns>Returns a validated version in the form major.minor.build or throws exceptions</returns>
        /// <exception cref="PackageManagerException"></exception>
        public abstract string ValidateExec();

        /// <exception cref="PackageManagerException"></exception>
        /// <exception cref="CandyShopException"></exception>
        public abstract void Upgrade(List<GenericPackage> packages);

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
        public async Task PinAsync(GenericPackage package)
        {
            await Task.Run(() => Pin(package));
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task UnpinAsync(GenericPackage package)
        {
            await Task.Run(() => Unpin(package));
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
        protected abstract void Pin(GenericPackage package);

        /// <exception cref="PackageManagerException"></exception>
        protected abstract void Unpin(GenericPackage package);

        /// <exception cref="CandyShopException"></exception>
        protected void EnableGsudoCache()
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
