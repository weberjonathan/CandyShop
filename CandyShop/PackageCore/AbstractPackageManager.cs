﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace CandyShop.PackageCore
{
    internal abstract class AbstractPackageManager
    {
        public required string Binary { get; set; }
        public required bool RequireManualElevation { get; set; }
        public abstract bool SupportsFetchingOutdated { get; }

        /// <exception cref="PackageManagerException"></exception>
        public abstract List<GenericPackage> FetchInstalled();

        /// <exception cref="PackageManagerException"></exception>
        public abstract List<GenericPackage> FetchOutdated();

        /// <exception cref="PackageManagerException"></exception>
        public abstract List<GenericPackage> FetchPinList();

        /// <exception cref="PackageManagerException"></exception>
        public abstract string FetchInfo(GenericPackage package);

        /// <exception cref="PackageManagerException"></exception>
        public abstract void Pin(GenericPackage package);

        /// <exception cref="PackageManagerException"></exception>
        public abstract void Unpin(GenericPackage package);

        /// <exception cref="PackageManagerException"></exception>
        /// <exception cref="CandyShopException"></exception>
        public abstract void Upgrade(List<GenericPackage> packages);

        /// <exception cref="PackageManagerException"></exception>
        public async Task<List<GenericPackage>> FetchInstalledAsync()
        {
            return await Task.Run(FetchInstalled);
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task<List<GenericPackage>> FetchOutdatedAsync()
        {
            return await Task.Run(FetchOutdated);
        }

        /// <exception cref="PackageManagerException"></exception>
        public async Task<List<GenericPackage>> FetchPinListAsync()
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

        protected PackageManagerProcess BuildProcess(string args, bool useGsudo = false)
        {
            return useGsudo ? new("gsudo", $"{Binary} {args}") : new (Binary, args);

        }
    }
}
