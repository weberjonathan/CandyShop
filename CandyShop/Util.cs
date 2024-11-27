using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Principal;

namespace CandyShop
{
    internal class PathUtil
    {
        private static readonly ReadOnlyCollection<char> pathChars = new([
            Path.DirectorySeparatorChar,
            Path.AltDirectorySeparatorChar,
            Path.PathSeparator,
            Path.VolumeSeparatorChar
        ]);

        /// <summary>
        /// Checks if a file exists or can be found on the PATH environment variable. Also see <see cref="FileExistsOnEnvPath(string, out string, string, EnvironmentVariableTarget)"/>
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extension">Extension to be added to the filename when looking on PATH, if the filename has no extension</param>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool FileExists(string path, string extension = ".exe", EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            return File.Exists(path) || FileExistsOnEnvPath(path, out _, extension, target);
        }

        /// <summary>
        /// Searches for a file in the directories on the PATH environment variable and checks whether the file exists.
        /// </summary>
        /// <param name="filename">e. g. <c>"winget"</c></param>
        /// <param name="path">Location of the resolved file</param>
        /// <param name="extension">Extension to be added to the filename, if the filename has no extension</param>
        /// <param name="target">Process, User or Machine</param>
        /// <returns></returns>
        public static bool FileExistsOnEnvPath(string filename, out string path, string extension = ".exe", EnvironmentVariableTarget target = EnvironmentVariableTarget.Process)
        {
            path = null;
            if (filename.Distinct().Where(pathChars.Contains).Any())
                return false;

            if (!Path.HasExtension(filename))
                filename += extension;

            path = Environment.GetEnvironmentVariable("PATH", target)
                .Split(Path.PathSeparator)
                .Select(dir => Path.Combine(dir, filename))
                .Where(File.Exists)
                .FirstOrDefault();

            return path != null;
        }
    }

    internal class Util
    {
        public static bool IsAdmin()
        {
            using WindowsIdentity identity = WindowsIdentity.GetCurrent();
            WindowsPrincipal principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static bool HasDots(string value, int n)
        {
            return value.Where(c => c.Equals('.')).Count() == n;
        }

        public static bool IsNumeric(string value, char allowed = '.')
        {
            return value.Where(c => !c.Equals(allowed) || !char.IsNumber(c)).Any();
        }
    }
}
