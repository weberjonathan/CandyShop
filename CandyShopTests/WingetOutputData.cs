namespace CandyShopTests
{
    internal class WingetOutputData
    {
        // TODO currently unused
        // unique output without version column if winget pins
        // only contains uninstalled packages (which can happen!)
        // Win 10 Build 19045.5371, winget v1.9.25200
        public static string PinnedOnlyUninstalledPackages => "\r" +
            "   - \r" +
            "   \\ \r" +
            "   | \r" +
            "   / \r" +
            "   - \r" +
            "   \\ \r" +
            "   | \r" +
            "                                                                                                                        \r" +
            "\r" +
            "   - \r" +
            "   \\ \r" +
            "   | \r" +
            "   / \r" +
            "   - \r" +
            "   \\ \r" +
            "                                                                                                                        \r" +
            "Name           ID                   Quelle Stecknadeltyp\r\n" +
            "---------------------------------------------------------\r\n" +
            "Docker Desktop Docker.DockerDesktop winget Pinning\r\n";
    }
}
