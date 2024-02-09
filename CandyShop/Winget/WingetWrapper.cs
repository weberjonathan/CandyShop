using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CandyShop.Winget
{
    public class WingetWrapper
    {
        public static async Task<List<WingetPackage>> FetchInstalledAsync()
        {
            return await Task.Run(FetchInstalled);
        }

        public static List<WingetPackage> FetchInstalled()
        {
            // TODO make sure dequeues do not throw bc of missing elements
            
            // launch process
            WingetProcess p = ProcessFactory.Winget($"list");
            p.ExecuteHidden();

            // remove download indicators from output by skipping to first mention of "Name"
            int i = 0;
            while (p.Output[i] != 'N')
            {
                i++;
            }
            
            Queue<string> output = new Queue<string>(p.Output[i..].Split(Environment.NewLine));

            string header = output.Dequeue();
            if (!header.StartsWith("Name"))
            {
                throw new WingetException(); // TODO invalid
            }

            int nameIndex = 0;
            int idIndex = GetNextColumnIndex(header, nameIndex);
            int versionIndex = GetNextColumnIndex(header, idIndex);
            int availableIndex = GetNextColumnIndex(header, versionIndex);
            int sourceIndex = GetNextColumnIndex(header, availableIndex);

             if (idIndex == 0 || versionIndex == 0 || availableIndex == 0 || sourceIndex == 0)
            {
                throw new WingetException(); // TODO why tf does this happen sometimes? 
            }

            string divider = output.Dequeue();
            if (!divider.StartsWith('-') && sourceIndex < divider.Length)
            {
                throw new WingetException(); // TODO
            }

            List<WingetPackage> packages = new List<WingetPackage>();
            while (output.Count > 0)
            {
                string row = output.Dequeue();
                if (String.IsNullOrEmpty(row))
                {
                    continue;
                }

                WingetPackage package = new WingetPackage();
                package.Name = row.Substring(nameIndex, idIndex).Trim();
                package.Id = row.Substring(idIndex, versionIndex - idIndex).Trim();
                package.Version = row.Substring(versionIndex, availableIndex - versionIndex).Trim();
                package.AvailableVersion = row[availableIndex..sourceIndex].Trim();
                package.Source = row.Substring(sourceIndex).Trim();

                packages.Add(package);
            }

            return packages;
        }

        public static async Task<List<WingetPackage>> FetchOutdatedAsync()
        {
            return await Task.Run(FetchOutdated);
        }

        public static List<WingetPackage> FetchOutdated()
        {
            List<WingetPackage> rtn = new List<WingetPackage>();

            // TODO

            return rtn;
        }

        public static async Task<string> FetchInfoAsync(WingetPackage package)
        {
            return await Task.Run(() => FetchInfo(package));
        }

        public static string FetchInfo(WingetPackage package)
        {
            if (!package.HasSource)
            {
                return "No sources available for the selected package.";
            }

            WingetProcess p = ProcessFactory.Winget($"show --id \"{package.Id}\" --exact");
            p.ExecuteHidden();
            
            int start = p.Output.IndexOf("Gefunden ") + "Gefunden ".Length;
            return start > 0 ? p.Output[start..] : p.Output;
        }

        public static void Upgrade(List<WingetPackage> packages)
        {
            foreach (WingetPackage pckg in packages) // TODO make cast safe?
            {
                // launch process
                string command = $"upgrade --id \"{pckg.Id}\" --silent --exact";
                Console.WriteLine($"> {command}");
                WingetProcess p = ProcessFactory.Winget(command); // TODO make silent an option for user
                p.Execute();
            }
        }

        private static int GetNextColumnIndex(string row, int startIndex)
        {
            int i = startIndex;
            while (i < row.Length && Char.IsLetterOrDigit(row[i]))
            {
                i++;
            }

            while (i < row.Length && Char.IsWhiteSpace(row[i]))
            {
                i++;
            }

            return i;
        }
    }
}
