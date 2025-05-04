using CandyShop.Properties;
using System;
using System.Reflection;
using System.Windows.Forms;

namespace CandyShop
{
    public class MetaInfo
    {
        private static readonly Version VersionObject = Assembly.GetExecutingAssembly().GetName().Version;
        private static readonly string Version = $"{VersionObject.Major}.{VersionObject.Minor}";

        public static string Name => Application.ProductName;

        public static string GetAppTitle() => $"{Name} v{Version}";

        public static string GetAppTitle(string packageSource) =>
            string.Format(LocaleEN.TEXT_APP_TITLE, Name, packageSource, Version);
    }
}
