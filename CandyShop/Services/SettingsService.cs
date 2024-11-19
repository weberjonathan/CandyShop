using CandyShop.PackageCore;
using CandyShop.Properties;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace CandyShop.Services
{

    // TODO version methods should be awaitable
    // TODO exception vs null return on error
    internal class SettingsService
    {
        public bool FileExists(string path, bool includePath = true)
        {
            return File.Exists(path) || (includePath && TryDetectOnPath(path, out _));
        }

        public bool TryDetectOnPath(string name, out string path)
        {
            path = null;

            if (Path.IsPathFullyQualified(name))
                return false;

            name = Path.GetFileName(name);
            if (!Path.HasExtension(name))
                name = $"{name}.exe";

            path = Environment.GetEnvironmentVariable("PATH") // TODO target
                .Split(';')
                .Select(dir => Path.Combine(dir, name))
                .Where(File.Exists)
                .FirstOrDefault();

            return path != null;
        }

        public string GetWingetVersion(string binary)
        {
            string version = null;
            var p = new PackageManagerProcess(binary, "--version");
            try
            {
                p.ExecuteHidden();
                if (p.ExitCode == 0)
                {
                    version = p.Output.Trim();
                    if (!version.StartsWith('v') || !HasDots(version, 2) || !IsNumeric(version[1..]))
                        version = null;
                }
                else
                {
                    throw new PackageManagerException();
                }
            }
            catch (Exception)
            {
                Log.Error(LocaleEN.ERROR_CHOCO_PATH);
            }

            return version;
        }

        public string GetChocoVersion(string binary)
        {
            string version = null;
            var p = new PackageManagerProcess(binary, "--version --limit-output");
            try
            {
                p.ExecuteHidden();
                if (p.ExitCode == 0)
                {
                    version = p.Output.Trim();
                    if (!HasDots(version, 2) || !IsNumeric(version))
                    {
                        version = null;
                    }
                    else
                    {
                        // TODO manager requires major version
                        version = $"v{version}";
                    }
                }
                else
                {
                    throw new PackageManagerException();
                }
            }
            catch (Exception)
            {
                Log.Error(LocaleEN.ERROR_CHOCO_PATH);
            }

            return version;
        }

        public string GetGsudoVersion(string binary)
        {
            string version = null;
            var p = new PackageManagerProcess(binary, "--version --limit-output");
            try
            {
                p.ExecuteHidden();
                if (p.ExitCode == 0)
                {
                    var output = p.Output.Trim().Split(' ');
                    if (output.Length > 1 && output[0].Equals("gsudo") && output[1].StartsWith('v') && HasDots(output[1], 2) && IsNumeric(output[1][1..]))
                    {
                        version = output[1];
                    }
                    else
                    {
                        version = null;
                    }
                }
                else
                {
                    throw new PackageManagerException();
                }
            }
            catch (Exception)
            {
                Log.Error(LocaleEN.ERROR_CHOCO_PATH);
            }

            return version;
        }

        private bool HasDots(string value, int n)
        {
            return value.Where(c => c.Equals('.')).Count() == n;
        }

        private bool IsNumeric(string value, char separator = '.')
        {
            return value.Where(c => !c.Equals(separator) || !char.IsNumber(c)).Any();
        }
    }
}
