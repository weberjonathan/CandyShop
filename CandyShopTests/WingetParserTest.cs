using CandyShop.PackageCore;

namespace CandyShopTests
{
    public class WingetParserTest
    {
        [Fact]
        public void Constructor_ParsePinnedNone_CheckResult()
        {
            // feature:  Winget output with no pinned packages
            // observed: Win 10 Build 19045.5371, winget v1.9.25200

            string wingetOutput = "\r" +
            "   - \r" +
            "   \\ \r" +
            "   | \r" +
            "   / \r" +
            "   - \r" +
            "   \\ \r" +
            "                                                                                                                        \r" +
            "\r" +
            "   - \r" +
            "   \\ \r" +
            "   | \r" +
            "   / \r" +
            "   - \r" +
            "                                                                                                                        \r" +
            "Es sind keine Pins konfiguriert.\r\n";

            var parser = new WingetParser(wingetOutput);
            Assert.False(parser.HasTable);
            Assert.Empty(parser.Columns);
            Assert.Empty(parser.Items);
        }

        [Fact]
        public void Constructor_ParsePinnedSome_CheckResult()
        {
            // feature:  Winget output with some pinned packages
            // observed: Win 10 Build 19045.5371, winget v1.9.25200

            string wingetOutput = "\r" +
                "   - \r" +
                "   \\ \r" +
                "   | \r" +
                "   / \r" +
                "   - \r" +
                "   \\ \r" +
                "                                                                                                                        \r" +
                "\r" +
                "   - \r" +
                "   \\ \r" +
                "   | \r" +
                "   / \r" +
                "   - \r" +
                "                                                                                                                        \r" +
                "Name                     ID                              Version              Quelle Stecknadeltyp\r\n" +
                "--------------------------------------------------------------------------------------------------\r\n" +
                "IntelliJ IDEA 2024.3     JetBrains.IntelliJIDEA.Ultimate 2024.3               winget Pinning\r\n" +
                "App-Installer            Microsoft.AppInstaller          1.24.25200.0         winget Pinning\r\n" +
                "balenaEtcher             Balena.Etcher                   1.19.25              winget Pinning\r\n" +
                "Docker Desktop           Docker.DockerDesktop                                 winget Pinning\r\n" +
                "Discord                  Discord.Discord                 1.0.9011             winget Pinning\r\n" +
                "Spotify                  Spotify.Spotify                 1.2.53.440.g7b2f582a winget Pinning\r\n" +
                "JetBrains Rider 2024.3.2 JetBrains.Rider                 2024.3.2             winget Pinning\r\n";

            string[] expectedCols = [
                "Name",
                "ID",
                "Version",
                "Quelle",
                "Stecknadeltyp"
            ];

            var parser = new WingetParser(wingetOutput);
            Assert.True(parser.HasTable);
            Assert.Equal(expectedCols, parser.Columns);
            Assert.Equal(7, parser.Items.Length);
            Assert.Equal(5, parser.Items[0].Length);
            Assert.Equal("IntelliJ IDEA 2024.3", parser.Items[0][0]);
            Assert.Equal("JetBrains.IntelliJIDEA.Ultimate", parser.Items[0][1]);
            Assert.Equal("2024.3", parser.Items[0][2]);
            Assert.Equal("winget", parser.Items[0][3]);
            Assert.Equal("Pinning", parser.Items[0][4]);
        }

        [Fact]
        public void Constructor_ParsePinnedSomeText_CheckResult()
        {
            // feature:  Winget output contains text in progress chars
            // observed: Win 10 Build 19045.5371, winget v1.9.25200

            string wingetOutput = "\r" +
                "   - \r" +
                "   \\ \r" +
                "   | \r" +
                "   / \r" +
                "   - \r" +
                "   \\ \r" +
                "   | \r" +
                "   / \r" +
                "   - \r" +
                "   \\ \r" +
                "   | \r" +
                "   / \r" +
                "   - \r" +
                "   \\ \r" +
                "                                                                                                                        \r" +
                "Fehler beim Versuch, die Quelle zu aktualisieren: winget\r\n" +
                "\r" +
                "   - \r" +
                "   \\ \r" +
                "   | \r" +
                "   / \r" +
                "   - \r" +
                "                                                                                                                        \r" +
                "Name                     ID                              Version              Quelle Stecknadeltyp\r\n" +
                "--------------------------------------------------------------------------------------------------\r\n" +
                "IntelliJ IDEA 2024.3     JetBrains.IntelliJIDEA.Ultimate 2024.3               winget Pinning\r\n" +
                "App-Installer            Microsoft.AppInstaller          1.24.25200.0         winget Pinning\r\n" +
                "balenaEtcher             Balena.Etcher                   1.19.25              winget Pinning\r\n" +
                "Docker Desktop           Docker.DockerDesktop                                 winget Pinning\r\n" +
                "Discord                  Discord.Discord                 1.0.9011             winget Pinning\r\n" +
                "Spotify                  Spotify.Spotify                 1.2.53.440.g7b2f582a winget Pinning\r\n" +
                "JetBrains Rider 2024.3.2 JetBrains.Rider                 2024.3.2             winget Pinning\r\n";

            string[] expectedCols = [
                "Name",
                "ID",
                "Version",
                "Quelle",
                "Stecknadeltyp"
            ];

            var parser = new WingetParser(wingetOutput);
            Assert.True(parser.HasTable);
            Assert.Equal(expectedCols, parser.Columns);
            Assert.Equal(7, parser.Items.Length);
            Assert.Equal(5, parser.Items[0].Length);
            Assert.Equal("IntelliJ IDEA 2024.3", parser.Items[0][0]);
            Assert.Equal("JetBrains.IntelliJIDEA.Ultimate", parser.Items[0][1]);
            Assert.Equal("2024.3", parser.Items[0][2]);
            Assert.Equal("winget", parser.Items[0][3]);
            Assert.Equal("Pinning", parser.Items[0][4]);
        }

        [Fact]
        public void Constructor_ParseInstalledSomeNoVersion_CheckResult()
        {
            // feature:  Winget output contains four columns, where in other configurations it usually contained five
            // observed: Win 11, winget v1.9.25200

            string wingetOutput = "\r" +
                "   - \r" +
                "                                                                                                                        \r" +
                "\r" +
                "   - \r" +
                "   \\ \r" +
                "                                                                                                                        \r" +
                "Name                                            ID                                               Version        Quelle\r\n" +
                "-----------------------------------------------------------------------------------------------------------------------\r\n" +
                "draw.io 26.0.9                                  JGraph.Draw                                      26.0.9         winget\r\n" +
                "NVM for Windows 1.1.12                          CoreyButler.NVMforWindows                        1.1.12         winget\r\n" +
                "7-Zip 24.09 (x64)                               7zip.7zip                                        24.09          winget\r\n" +
                "OpenAudible 4.5.1                               OpenAudible.OpenAudible                          4.5.1          winget\r\n" +
                "Audacity 3.7.1                                  Audacity.Audacity                                3.7.1          winget\r\n" +
                "CPUID HWMonitor 1.55                            CPUID.HWMonitor                                  1.55           winget\r\n" +
                "Docker Desktop                                  Docker.DockerDesktop                             4.37.1         winget\r\n" +
                "GIMP 2.10.38-1                                  GIMP.GIMP                                        2.10.38        winget\r\n" +
                "Git                                             ARP\\Machine\\X64\\Git_is1                          2.47.1.2       \r\n" +
                "HWiNFO® 64                                      REALiX.HWiNFO                                    8.20           winget\r\n" +
                "MSEdgeRedirect                                  rcmaehl.MSEdgeRedirect                           0.7.5.3        winget\r\n" +
                "Mozilla Firefox (x64 de)                        Mozilla.Firefox                                  134.0.2        winget\r\n" +
                "Mozilla Thunderbird (x64 de)                    Mozilla.Thunderbird.de                           128.6.0        winget\r\n" +
                "Mozilla Maintenance Service                     ARP\\Machine\\X64\\MozillaMaintenanceService        128.6.0        \r\n" +
                "Notepad++ (64-bit x64)                          Notepad++.Notepad++                              8.7.6          winget\r\n" +
                "ONLYOFFICE Desktop Editors 8.2.2 (x64)          ONLYOFFICE.DesktopEditors                        8.2.2          winget\r\n" +
                "Oculus                                          Meta.Oculus                                      <3             winget\r\n" +
                "Timberborn                                      ARP\\Machine\\X64\\Steam App 1062090                Unknown        \r\n" +
                "Automobilista 2                                 ARP\\Machine\\X64\\Steam App 1066890                Unknown        \r\n" +
                "WorldBox - God Simulator                        ARP\\Machine\\X64\\Steam App 1206560                Unknown        \r\n" +
                "ELDEN RING                                      ARP\\Machine\\X64\\Steam App 1245620                Unknown        \r\n" +
                "Portal Reloaded                                 ARP\\Machine\\X64\\Steam App 1255980                Unknown        \r\n" +
                "Orcs Must Die! 2                                ARP\\Machine\\X64\\Steam App 201790                 Unknown        \r\n" +
                "Hotline Miami                                   ARP\\Machine\\X64\\Steam App 219150                 Unknown        \r\n" +
                "Game Dev Tycoon                                 ARP\\Machine\\X64\\Steam App 239820                 Unknown        \r\n" +
                "Assetto Corsa                                   ARP\\Machine\\X64\\Steam App 244210                 Unknown        \r\n" +
                "Burnout Paradise: The Ultimate Box              ARP\\Machine\\X64\\Steam App 24740                  Unknown        \r\n" +
                "SteamVR                                         ARP\\Machine\\X64\\Steam App 250820                 Unknown        \r\n" +
                "Split/Second                                    ARP\\Machine\\X64\\Steam App 297860                 Unknown        \r\n" +
                "Plants vs. Zombies: Game of the Year            ARP\\Machine\\X64\\Steam App 3590                   Unknown        \r\n" +
                "Project CARS 2                                  ARP\\Machine\\X64\\Steam App 378860                 Unknown        \r\n" +
                "Kingdom Come: Deliverance                       ARP\\Machine\\X64\\Steam App 379430                 Unknown        \r\n" +
                "Borderlands 3                                   ARP\\Machine\\X64\\Steam App 397540                 Unknown        \r\n" +
                "Deep Rock Galactic                              ARP\\Machine\\X64\\Steam App 548430                 Unknown        \r\n" +
                "Golf It!                                        ARP\\Machine\\X64\\Steam App 571740                 Unknown        \r\n" +
                "Portal 2                                        ARP\\Machine\\X64\\Steam App 620                    Unknown        \r\n" +
                "Beat Saber                                      ARP\\Machine\\X64\\Steam App 620980                 Unknown        \r\n" +
                "Slay the Spire                                  ARP\\Machine\\X64\\Steam App 646570                 Unknown        \r\n" +
                "DiRT Rally 2.0                                  ARP\\Machine\\X64\\Steam App 690790                 Unknown        \r\n" +
                "Counter-Strike: Global Offensive                ARP\\Machine\\X64\\Steam App 730                    Unknown        \r\n" +
                "STAR WARSTM Episode I Racer                     ARP\\Machine\\X64\\Steam App 808910                 Unknown        \r\n" +
                "VLC media player                                VideoLAN.VLC                                     3.0.21         winget\r\n" +
                "Lunacy                                          Icons8.Lunacy                                    10.11.0.0      winget\r\n" +
                "Microsoft Visual C++ 2010  x64 Redistributable… Microsoft.VCRedist.2010.x64                      10.0.40219     winget\r\n" +
                "Microsoft Update Health Tools                   ARP\\Machine\\X64\\{1FC1A6C2-576E-489A-9B4A-92D21F… 3.74.0.0       \r\n" +
                "OpenVPN Connect                                 OpenVPNTechnologies.OpenVPNConnect               3.6.0          winget\r\n" +
                "Zoom Workplace (64-bit)                         Zoom.Zoom                                        6.3.56144      winget\r\n" +
                "AMS2CM                                          ARP\\Machine\\X64\\{398584DF-A0E2-4C87-85DA-C127B4… 0.3.0.0        \r\n" +
                "Microsoft Server Speech Platform Runtime (x64)  ARP\\Machine\\X64\\{3B433087-E62E-4BF5-97F9-4AF6E1… 11.0.7400.345  \r\n" +
                "Windows-PC-Integritätsprüfung                   Microsoft.WindowsPCHealthCheck                   3.7.2204.15001 winget\r\n" +
                "CORSAIR iCUE 4 Software                         Corsair.iCUE.4                                   4.33.138       winget\r\n" +
                "Logitech G HUB                                  Logitech.GHUB                                    2024.7.625196  winget\r\n" +
                "Nextcloud                                       Nextcloud.NextcloudDesktop                       3.15.3         winget\r\n" +
                "AusweisApp                                      Governikus.AusweisApp                            2.2.2          winget\r\n" +
                "NVIDIA Grafiktreiber 566.36                     ARP\\Machine\\X64\\{B2FE1952-0186-46C3-BAEC-A80AA3… 566.36         \r\n" +
                "NVIDIA GeForce Experience 3.28.0.417            ARP\\Machine\\X64\\{B2FE1952-0186-46C3-BAEC-A80AA3… 3.28.0.417     \r\n" +
                "NVIDIA PhysX-Systemsoftware 9.23.1019           Nvidia.PhysX                                     9.23.1019      winget\r\n" +
                "NVIDIA FrameView SDK 1.3.8513.32290073          ARP\\Machine\\X64\\{B2FE1952-0186-46C3-BAEC-A80AA3… 1.3.8513.3229… \r\n" +
                "NVIDIA HD-Audiotreiber 1.4.2.6                  ARP\\Machine\\X64\\{B2FE1952-0186-46C3-BAEC-A80AA3… 1.4.2.6        \r\n" +
                "WinDirStat                                      ARP\\Machine\\X64\\{D1A4F34E-BFF2-4AF9-A682-CCE198… 2.2.0          \r\n" +
                "Java(TM) SE Development Kit 17.0.12 (64-bit)    Oracle.JDK.17                                    17.0.12.0      winget\r\n" +
                "Update for x64-based Windows Systems (KB500171… ARP\\Machine\\X64\\{DA80A019-4C3B-4DAA-ACA1-6937D7… 8.94.0.0       \r\n" +
                "Paint.NET                                       dotPDN.PaintDotNet                               5.1.2          winget\r\n" +
                "Microsoft Visual Studio Code                    Microsoft.VisualStudioCode                       1.96.4         winget\r\n" +
                "gsudo v2.5.1                                    gerardog.gsudo                                   2.5.1          winget\r\n" +
                "Windows Subsystem for Linux Update              ARP\\Machine\\X64\\{F8474A47-8B5D-4466-ACE3-78EAB3… 5.10.102.1     \r\n" +
                "Star Wars Episode 1 Racer                       ARP\\Machine\\X64\\{ac41225e-dadc-45c1-9f7e-00e45a… Unknown        \r\n" +
                "Cyberpunk 2077: Phantom Liberty                 ARP\\Machine\\X86\\1256837418_is1                   2.02           \r\n" +
                "Cyberpunk 2077                                  ARP\\Machine\\X86\\1423049311_is1                   2.02           \r\n" +
                "DroidCam Client                                 dev47apps.DroidCam                               6.5.2          winget\r\n" +
                "EventGhost 0.5.0-rc6                            ARP\\Machine\\X86\\EventGhost_is1                   0.5.0-rc6      \r\n" +
                "GNU Privacy Guard                               GnuPG.GnuPG                                      2.4.7          winget\r\n" +
                "Gpg4win (4.4.0)                                 GnuPG.Gpg4win                                    4.4.0          winget\r\n" +
                "IntelliJ IDEA Community Edition 2024.3.2.1      JetBrains.IntelliJIDEA.Community                 2024.3.2.1     winget\r\n" +
                "MEGAsync                                        Mega.MEGASync                                    Unknown        winget\r\n" +
                "Microsoft Edge                                  Microsoft.Edge                                   132.0.2957.127 winget\r\n" +
                "OpenAL                                          ARP\\Machine\\X86\\OpenAL                           Unknown        \r\n" +
                "Origin                                          ElectronicArts.Origin                            10.5.129.55742 winget\r\n" +
                "Rockstar Games Launcher                         ARP\\Machine\\X86\\Rockstar Games Launcher          1.0.71.1428    \r\n" +
                "Rockstar Games Social Club                      ARP\\Machine\\X86\\Rockstar Games Social Club       2.1.7.0        \r\n" +
                "Steam                                           Valve.Steam                                      2.10.91.91     winget\r\n" +
                "Ubisoft Connect                                 Ubisoft.Connect                                  159.1.11430    winget\r\n" +
                "Star Wars Outlaws                               ARP\\Machine\\X86\\Uplay Install 17903              Unknown        \r\n" +
                "Trackmania Turbo                                ARP\\Machine\\X86\\Uplay Install 2070               Unknown        \r\n" +
                "Trackmania                                      ARP\\Machine\\X86\\Uplay Install 5595               Unknown        \r\n" +
                "Plex Media Player                               Plex.PlexMediaPlayer                             2.58.0         winget\r\n" +
                "Microsoft Visual C++ 2013 Redistributable (x64… Microsoft.VCRedist.2013.x64                      12.0.40664.0   winget\r\n" +
                "Microsoft Visual C++ 2013 Redistributable (x64… Microsoft.VCRedist.2013.x64                      12.0.30501.0   winget\r\n" +
                "Microsoft Windows Desktop Runtime - 9.0.1 (x64) Microsoft.DotNet.DesktopRuntime.9                9.0.1          winget\r\n" +
                "Microsoft Windows Desktop Runtime - 9.0.1 (x86) Microsoft.DotNet.DesktopRuntime.9                9.0.1          winget\r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{07FC9CAD-FCEC-4186-BB83-EF7CCC… 11.0.7400.336  \r\n" +
                "Microsoft Windows Desktop Runtime - 3.1.32 (x6… Microsoft.DotNet.DesktopRuntime.3_1              3.1.32         winget\r\n" +
                "Microsoft Windows Desktop Runtime - 3.1.32 (x8… Microsoft.DotNet.DesktopRuntime.3_1              3.1.32         winget\r\n" +
                "Windows 11-Installationsassistent               Microsoft.WindowsInstallationAssistant           1.4.19041.5003 winget\r\n" +
                "PowerToys (Preview) x64                         Microsoft.PowerToys                              0.88.0         winget\r\n" +
                "Microsoft GameInput                             ARP\\Machine\\X86\\{1F2B6AF3-C260-8666-5950-E3FEDB… 10.1.22621.30… \r\n" +
                "Microsoft Visual C++ 2012 Redistributable (x86… Microsoft.VCRedist.2012.x86                      11.0.61030.0   winget\r\n" +
                "Microsoft Windows Desktop Runtime - 7.0.20 (x6… Microsoft.DotNet.DesktopRuntime.7                7.0.20         winget\r\n" +
                "Microsoft Windows Desktop Runtime - 7.0.20 (x8… Microsoft.DotNet.DesktopRuntime.7                7.0.20         winget\r\n" +
                "Microsoft Server Speech Recognition Language -… ARP\\Machine\\X86\\{3B06AC90-DE68-44A9-95EB-0A3C1A… 11.0.7400.335  \r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{48CEC0A3-AE10-4EE3-AC62-76D3D5… 11.0.7400.336  \r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{4CC174AA-25BC-46FF-B1E2-13B24A… 11.0.7400.336  \r\n" +
                "Chocolatey GUI                                  Chocolatey.ChocolateyGUI                         2.1.1.0        winget\r\n" +
                "Epic Online Services                            ARP\\Machine\\X86\\{57A956AB-4BCC-45C6-9B40-957E4E… 2.0.44.0       \r\n" +
                "Microsoft Visual C++ 2013 Redistributable (x86… Microsoft.VCRedist.2013.x86                      12.0.40660.0   winget\r\n" +
                "Microsoft Visual C++ 2013 Redistributable (x86… Microsoft.VCRedist.2013.x86                      12.0.30501.0   winget\r\n" +
                "Microsoft Visual C++ 2005 Redistributable       Microsoft.VCRedist.2005.x86                      8.0.61001      winget\r\n" +
                "Microsoft Windows Desktop Runtime - 8.0.12 (x8… Microsoft.DotNet.DesktopRuntime.8                8.0.12         winget\r\n" +
                "Microsoft Windows Desktop Runtime - 8.0.12 (x6… Microsoft.DotNet.DesktopRuntime.8                8.0.12         winget\r\n" +
                "GOG GALAXY                                      GOG.Galaxy                                       Unknown        winget\r\n" +
                "Volksverschlüsselungs-Software v1.22-6          ARP\\Machine\\X86\\{75D939AB-C78E-4EA3-99E6-404FB3… v1.22-6        \r\n" +
                "Microsoft .NET Core Runtime - 3.1.32 (x64)      Microsoft.DotNet.Runtime.3_1                     3.1.32         winget\r\n" +
                "Microsoft .NET Core Runtime - 3.1.32 (x86)      Microsoft.DotNet.Runtime.3_1                     3.1.32         winget\r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{7D179500-CA0C-4456-B624-C15876… 11.0.7400.336  \r\n" +
                "Microsoft Visual C++ 2015-2022 Redistributable… Microsoft.VCRedist.2015+.x64                     14.42.34433.0  winget\r\n" +
                "Microsoft .NET SDK 6.0.428 (x64)                Microsoft.DotNet.SDK.6                           6.0.428        winget\r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{898AA67F-99B8-4C7F-9611-B11F98… 11.0.7413.611  \r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{8AAA44BB-487E-4D01-AF76-484ACB… 11.0.7400.336  \r\n" +
                "Epic Games Launcher                             EpicGames.EpicGamesLauncher                      1.3.128.0      winget\r\n" +
                "F1® 23                                          ARP\\Machine\\X86\\{8EC807D1-1401-4E28-8FA8-104727… 1.0.108.2038   \r\n" +
                "STAR WARSTM BattlefrontTM II                    ARP\\Machine\\X86\\{8a882ce0-0c0b-4eb2-850c-28ebad… 1.1.8.16162    \r\n" +
                "CertsUpdater version 1.5                        ARP\\Machine\\X86\\{90DE7E86-6F5A-4125-9EC5-D95093… 1.5            \r\n" +
                "Python Launcher                                 Python.Launcher                                  > 3.12.0       winget\r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{969D900A-3481-4A77-B888-D24160… 11.0.7400.336  \r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{998D5259-3BED-4710-98FF-D63387… 11.0.7400.336  \r\n" +
                "Microsoft Visual C++ 2008 Redistributable - x8… ARP\\Machine\\X86\\{9A25302D-30C0-39D9-BD6F-21E6EC… 9.0.30729      \r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{9C5505DA-F9C1-46CB-9F8F-AC38F8… 11.0.7400.336  \r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{A0186231-0A8B-455A-8A25-B64AAB… 11.0.7400.336  \r\n" +
                "BurnoutTM Paradise Remastered                   ARP\\Machine\\X86\\{ADF3783C-C4B7-46A0-A0A6-EC4CA3… 1.0.0.0        \r\n" +
                "STAR WARS Jedi - SurvivorTM                     ARP\\Machine\\X86\\{B9CBE70C-C93E-467A-B112-D12665… 1.0.0.9        \r\n" +
                "Microsoft Server Speech Recognition Language -… ARP\\Machine\\X86\\{BAD2A75A-1708-47BA-A498-20890D… 11.0.7400.335  \r\n" +
                "Microsoft Server Speech Recognition Language -… ARP\\Machine\\X86\\{BEFB9378-5E88-4266-8EB1-C92869… 11.0.7400.335  \r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{E8F3B154-03CE-4120-8B9D-9E83ED… 11.0.7400.336  \r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{EDA8693D-9E82-4FD1-98C8-0DC4F9… 11.0.7400.336  \r\n" +
                "Microsoft Visual C++ 2010  x86 Redistributable… Microsoft.VCRedist.2010.x86                      10.0.40219     winget\r\n" +
                "Need for SpeedTM Hot Pursuit Remastered         ARP\\Machine\\X86\\{F28231EF-0D0C-41AD-9020-2B993F… 1.0.0.23890    \r\n" +
                "Kinect for Windows Speech Recognition Language… ARP\\Machine\\X86\\{F49AF755-A5C3-4252-A190-5772B2… 11.0.7400.336  \r\n" +
                "Microsoft Server Speech Recognition Language -… ARP\\Machine\\X86\\{F6B5EB21-0ABF-487C-B9A9-D9DB25… 11.0.7400.335  \r\n" +
                "Microsoft Visual C++ 2012 Redistributable (x64… Microsoft.VCRedist.2012.x64                      11.0.61030.0   winget\r\n" +
                "Microsoft Visual C++ 2015-2022 Redistributable… Microsoft.VCRedist.2015+.x86                     14.42.34433.0  winget\r\n" +
                "EA app                                          ElectronicArts.EADesktop                         13.380.0.5893  winget\r\n" +
                "Bitwarden                                       Bitwarden.Bitwarden                              2025.1.3       winget\r\n" +
                "Signal 7.40.1                                   OpenWhisperSystems.Signal                        7.40.1         winget\r\n" +
                "MQTT Explorer 0.3.5                             ARP\\User\\X64\\8119b299-9e43-554d-ab01-0b529cdd5b… 0.3.5          \r\n" +
                "Discord                                         Discord.Discord                                  1.0.9147       winget\r\n" +
                "Microsoft OneDrive                              Microsoft.OneDrive                               25.004.0109.0… winget\r\n" +
                "Raspberry Pi Imager                             RaspberryPiFoundation.RaspberryPiImager          1.9.0          winget\r\n" +
                "Microsoft Teams classic                         Microsoft.Teams.Classic                          1.8.00.1362    winget\r\n" +
                "SignalRgb                                       WhirlwindFX.SignalRgb                            2.3.35         winget\r\n" +
                "balenaEtcher                                    Balena.Etcher                                    1.19.25        winget\r\n" +
                "Insomnia                                        Insomnia.Insomnia                                10.3.0         winget\r\n" +
                "Python 3.11.9 (64-bit)                          Python.Python.3.11                               3.11.9         winget\r\n" +
                "Candy Shop                                      ARP\\User\\X64\\{3AB9D1A3-D54B-486C-8178-454BB98B0… 0.11           \r\n" +
                "Telegram Desktop                                Telegram.TelegramDesktop                         5.10.7         winget\r\n" +
                "REDlauncher                                     ARP\\User\\X64\\{7258BA11-600C-430E-A759-27E2C691A… Unknown        \r\n" +
                "Python 3.12.8 (64-bit)                          Python.Python.3.12                               3.12.8         winget\r\n" +
                "MOZA Pit House                                  ARP\\User\\X64\\{e51f3542-b079-4889-8aa6-4ef424536… 1.0.1          \r\n" +
                "Python 3.9.13 (64-bit)                          Python.Python.3.9                                3.9.13         winget\r\n" +
                "WhatsApp                                        MSIX\\5319275A.WhatsAppDesktop_2.2504.2.0_x64__c… 2.2504.2.0     \r\n" +
                "Ubuntu 24.04.1 LTS                              Canonical.Ubuntu.2404                            2404.1.26.0    winget\r\n" +
                "Microsoft Clipchamp                             MSIX\\Clipchamp.Clipchamp_3.1.12020.0_neutral__y… 3.1.12020.0    \r\n" +
                "Microsoft Teams                                 Microsoft.Teams                                  24335.208.331… winget\r\n" +
                "Minecraft Launcher                              MSIX\\Microsoft.4297127D64EC6_2.1.3.0_x64__8weky… 2.1.3.0        \r\n" +
                "AV1 Video Extension                             MSIX\\Microsoft.AV1VideoExtension_1.3.4.0_x64__8… 1.3.4.0        \r\n" +
                "AVC Encoder Video Extension                     MSIX\\Microsoft.AVCEncoderVideoExtension_1.1.9.0… 1.1.9.0        \r\n" +
                "Verbesserungen an der Windows-Anwendungskompat… MSIX\\Microsoft.ApplicationCompatibilityEnhancem… 1.2411.16.0    \r\n" +
                "Nachrichten                                     MSIX\\Microsoft.BingNews_4.55.62231.0_x64__8weky… 4.55.62231.0   \r\n" +
                "Websuche von Microsoft Bing                     MSIX\\Microsoft.BingSearch_1.1.3.0_x64__8wekyb3d… 1.1.3.0        \r\n" +
                "MSN Wetter                                      MSIX\\Microsoft.BingWeather_4.54.63007.0_x64__8w… 4.54.63007.0   \r\n" +
                "App-Installer                                   Microsoft.AppInstaller                           1.24.25200.0   winget\r\n" +
                "DirectX                                         MSIX\\Microsoft.DirectXRuntime_9.29.1974.0_x64__… 9.29.1974.0    \r\n" +
                "DirectX                                         MSIX\\Microsoft.DirectXRuntime_9.29.1974.0_x86__… 9.29.1974.0    \r\n" +
                "Xbox                                            MSIX\\Microsoft.GamingApp_2501.1001.3.0_x64__8we… 2501.1001.3.0  \r\n" +
                "Gaming Services                                 MSIX\\Microsoft.GamingServices_26.95.25001.0_x64… 26.95.25001.0  \r\n" +
                "Hilfe anfordern                                 MSIX\\Microsoft.GetHelp_10.2409.22951.0_x64__8we… 10.2409.22951… \r\n" +
                "HEIF Image Extensions                           MSIX\\Microsoft.HEIFImageExtension_1.2.3.0_x64__… 1.2.3.0        \r\n" +
                "HEVC-Videoerweiterungen vom Gerätehersteller    MSIX\\Microsoft.HEVCVideoExtension_2.2.9.0_x64__… 2.2.9.0        \r\n" +
                "Deutsch Local Experience Pack                   MSIX\\Microsoft.LanguageExperiencePackde-DE_2610… 26100.22.42.0  \r\n" +
                "MPEG-2-Videoerweiterung                         MSIX\\Microsoft.MPEG2VideoExtension_1.0.61931.0_… 1.0.61931.0    \r\n" +
                "Paint 3D                                        MSIX\\Microsoft.MSPaint_6.2410.13017.0_x64__8wek… 6.2410.13017.0 \r\n" +
                "3D-Viewer                                       MSIX\\Microsoft.Microsoft3DViewer_7.2407.16012.0… 7.2407.16012.0 \r\n" +
                "Microsoft Edge                                  MSIX\\Microsoft.MicrosoftEdge.Stable_132.0.2957.… 132.0.2957.127 \r\n" +
                "Microsoft 365 Copilot                           MSIX\\Microsoft.MicrosoftOfficeHub_18.2501.1284.… 18.2501.1284.0 \r\n" +
                "Microsoft-Kurznotizen                           MSIX\\Microsoft.MicrosoftStickyNotes_6.1.4.0_x64… 6.1.4.0        \r\n" +
                "Mixed Reality-Portal                            MSIX\\Microsoft.MixedReality.Portal_2000.21051.1… 2000.21051.12… \r\n" +
                "Microsoft .Net Native Framework Package 1.7     MSIX\\Microsoft.NET.Native.Framework.1.7_1.7.274… 1.7.27413.0    \r\n" +
                "Microsoft .Net Native Framework Package 1.7     MSIX\\Microsoft.NET.Native.Framework.1.7_1.7.274… 1.7.27413.0    \r\n" +
                "Microsoft .Net Native Framework Package 2.2     MSIX\\Microsoft.NET.Native.Framework.2.2_2.2.295… 2.2.29512.0    \r\n" +
                "Microsoft .Net Native Framework Package 2.2     MSIX\\Microsoft.NET.Native.Framework.2.2_2.2.295… 2.2.29512.0    \r\n" +
                "Microsoft .Net Native Runtime Package 1.7       MSIX\\Microsoft.NET.Native.Runtime.1.7_1.7.27422… 1.7.27422.0    \r\n" +
                "Microsoft .Net Native Runtime Package 1.7       MSIX\\Microsoft.NET.Native.Runtime.1.7_1.7.27422… 1.7.27422.0    \r\n" +
                "Microsoft .Net Native Runtime Package 2.2       MSIX\\Microsoft.NET.Native.Runtime.2.2_2.2.28604… 2.2.28604.0    \r\n" +
                "Microsoft .Net Native Runtime Package 2.2       MSIX\\Microsoft.NET.Native.Runtime.2.2_2.2.28604… 2.2.28604.0    \r\n" +
                "OneNote for Windows 10                          MSIX\\Microsoft.Office.OneNote_16001.14326.22094… 16001.14326.2… \r\n" +
                "OneDrive                                        MSIX\\Microsoft.OneDriveSync_25004.109.2.0_neutr… 25004.109.2.0  \r\n" +
                "Outlook for Windows                             MSIX\\Microsoft.OutlookForWindows_1.2025.122.200… 1.2025.122.200 \r\n" +
                "Paint                                           MSIX\\Microsoft.Paint_11.2411.471.0_x64__8wekyb3… 11.2411.471.0  \r\n" +
                "Microsoft Kontakte                              MSIX\\Microsoft.People_10.2202.100.0_x64__8wekyb… 10.2202.100.0  \r\n" +
                "Power Automate                                  MSIX\\Microsoft.PowerAutomateDesktop_1.0.1350.0_… 1.0.1350.0     \r\n" +
                "PowerToys FileLocksmith Context Menu            MSIX\\Microsoft.PowerToys.FileLocksmithContextMe… 0.88.0.0       \r\n" +
                "PowerToys ImageResizer Context Menu             MSIX\\Microsoft.PowerToys.ImageResizerContextMen… 0.88.0.0       \r\n" +
                "PowerToys PowerRename Context Menu              MSIX\\Microsoft.PowerToys.PowerRenameContextMenu… 0.88.0.0       \r\n" +
                "Raw Image Extension                             MSIX\\Microsoft.RawImageExtension_2.5.3.0_x64__8… 2.5.3.0        \r\n" +
                "Snipping Tool                                   MSIX\\Microsoft.ScreenSketch_11.2409.25.0_x64__8… 11.2409.25.0   \r\n" +
                "Windows-Sicherheit                              MSIX\\Microsoft.SecHealthUI_1000.27703.1006.0_x6… 1000.27703.10… \r\n" +
                "Microsoft Engagement Framework                  MSIX\\Microsoft.Services.Store.Engagement_10.0.2… 10.0.23012.0   \r\n" +
                "Microsoft Engagement Framework                  MSIX\\Microsoft.Services.Store.Engagement_10.0.2… 10.0.23012.0   \r\n" +
                "Skype                                           MSIX\\Microsoft.SkypeApp_15.134.3202.0_x64__kzf8… 15.134.3202.0  \r\n" +
                "Start Experiences-App                           MSIX\\Microsoft.StartExperiencesApp_1.1.235.0_x6… 1.1.235.0      \r\n" +
                "Host der Store-Benutzeroberfläche               MSIX\\Microsoft.StorePurchaseApp_22411.1401.1.0_… 22411.1401.1.0 \r\n" +
                "Microsoft To Do                                 MSIX\\Microsoft.Todos_0.114.7122.0_x64__8wekyb3d… 0.114.7122.0   \r\n" +
                "Microsoft.UI.Xaml.2.0                           MSIX\\Microsoft.UI.Xaml.2.0_2.1810.18004.0_x64__… 2.1810.18004.0 \r\n" +
                "Microsoft.UI.Xaml.2.0                           MSIX\\Microsoft.UI.Xaml.2.0_2.1810.18004.0_x86__… 2.1810.18004.0 \r\n" +
                "Microsoft.UI.Xaml.2.1                           MSIX\\Microsoft.UI.Xaml.2.1_2.11906.6001.0_x64__… 2.11906.6001.0 \r\n" +
                "Microsoft.UI.Xaml.2.1                           MSIX\\Microsoft.UI.Xaml.2.1_2.11906.6001.0_x86__… 2.11906.6001.0 \r\n" +
                "Microsoft.UI.Xaml.2.3                           MSIX\\Microsoft.UI.Xaml.2.3_2.32002.13001.0_x64_… 2.32002.13001… \r\n" +
                "Microsoft.UI.Xaml.2.3                           MSIX\\Microsoft.UI.Xaml.2.3_2.32002.13001.0_x86_… 2.32002.13001… \r\n" +
                "Microsoft.UI.Xaml.2.4                           MSIX\\Microsoft.UI.Xaml.2.4_2.42007.9001.0_x64__… 2.42007.9001.0 \r\n" +
                "Microsoft.UI.Xaml.2.4                           MSIX\\Microsoft.UI.Xaml.2.4_2.42007.9001.0_x86__… 2.42007.9001.0 \r\n" +
                "Microsoft.UI.Xaml.2.7                           Microsoft.UI.Xaml.2.7                            7.2409.9001.0  winget\r\n" +
                "Microsoft.UI.Xaml.2.7                           Microsoft.UI.Xaml.2.7                            7.2409.9001.0  winget\r\n" +
                "Microsoft.UI.Xaml.2.8                           Microsoft.UI.Xaml.2.8                            8.2310.30001.0 winget\r\n" +
                "Microsoft.UI.Xaml.2.8                           Microsoft.UI.Xaml.2.8                            8.2310.30001.0 winget\r\n" +
                "Microsoft.UI.Xaml.2.8                           Microsoft.UI.Xaml.2.8                            8.2306.22001.0 winget\r\n" +
                "Microsoft Visual C++ 2012 UWP Desktop Runtime … MSIX\\Microsoft.VCLibs.110.00.UWPDesktop_11.0.61… 11.0.61135.0   \r\n" +
                "Microsoft Visual C++ 2012 UWP Desktop Runtime … MSIX\\Microsoft.VCLibs.110.00.UWPDesktop_11.0.61… 11.0.61135.0   \r\n" +
                "Microsoft Visual C++ 2013 UWP Desktop Runtime … MSIX\\Microsoft.VCLibs.120.00.UWPDesktop_12.0.40… 12.0.40653.0   \r\n" +
                "Microsoft Visual C++ 2013 UWP Desktop Runtime … MSIX\\Microsoft.VCLibs.120.00.UWPDesktop_12.0.40… 12.0.40653.0   \r\n" +
                "Microsoft Visual C++ 2015 UWP Desktop Runtime … Microsoft.VCLibs.Desktop.14                      14.0.33728.0   winget\r\n" +
                "Microsoft Visual C++ 2015 UWP Desktop Runtime … Microsoft.VCLibs.Desktop.14                      14.0.33728.0   winget\r\n" +
                "Microsoft Visual C++ 2015 UWP Desktop Runtime … Microsoft.VCLibs.Desktop.14                      14.0.33519.0   winget\r\n" +
                "Microsoft Visual C++ 2015 UWP Desktop Runtime … Microsoft.VCLibs.Desktop.14                      14.0.33321.0   winget\r\n" +
                "Microsoft Visual C++ 2015 UWP Desktop Runtime … Microsoft.VCLibs.Desktop.14                      14.0.32530.0   winget\r\n" +
                "Microsoft Visual C++ 2015 UWP Runtime Package   MSIX\\Microsoft.VCLibs.140.00_14.0.30704.0_x64__… 14.0.30704.0   \r\n" +
                "Microsoft Visual C++ 2015 UWP Runtime Package   MSIX\\Microsoft.VCLibs.140.00_14.0.32530.0_x64__… 14.0.32530.0   \r\n" +
                "Microsoft Visual C++ 2015 UWP Runtime Package   MSIX\\Microsoft.VCLibs.140.00_14.0.33321.0_x64__… 14.0.33321.0   \r\n" +
                "Microsoft Visual C++ 2015 UWP Runtime Package   MSIX\\Microsoft.VCLibs.140.00_14.0.33321.0_x86__… 14.0.33321.0   \r\n" +
                "Microsoft Visual C++ 2015 UWP Runtime Package   MSIX\\Microsoft.VCLibs.140.00_14.0.33519.0_x64__… 14.0.33519.0   \r\n" +
                "Microsoft Visual C++ 2015 UWP Runtime Package   MSIX\\Microsoft.VCLibs.140.00_14.0.33519.0_x86__… 14.0.33519.0   \r\n" +
                "VP9 Video Extensions                            MSIX\\Microsoft.VP9VideoExtensions_1.2.2.0_x64__… 1.2.2.0        \r\n" +
                "Webmedienerweiterungen                          MSIX\\Microsoft.WebMediaExtensions_1.1.1295.0_x6… 1.1.1295.0     \r\n" +
                "Webp Image Extensions                           MSIX\\Microsoft.WebpImageExtension_1.1.1711.0_x6… 1.1.1711.0     \r\n" +
                "Widgets Platform Runtime                        MSIX\\Microsoft.WidgetsPlatformRuntime_1.6.1.0_x… 1.6.1.0        \r\n" +
                "Windows App Runtime DDLM 5001.311.2039.0-x6     MSIX\\Microsoft.WinAppRuntime.DDLM.5001.311.2039… 5001.311.2039… \r\n" +
                "Windows App Runtime DDLM 5001.311.2039.0-x8     MSIX\\Microsoft.WinAppRuntime.DDLM.5001.311.2039… 5001.311.2039… \r\n" +
                "Dev Home                                        Microsoft.DevHome                                0.2000.758.0   winget\r\n" +
                "Microsoft Fotos                                 MSIX\\Microsoft.Windows.Photos_2024.11120.5010.0… 2024.11120.50… \r\n" +
                "Windows-Uhr                                     MSIX\\Microsoft.WindowsAlarms_11.2411.2.0_x64__8… 11.2411.2.0    \r\n" +
                "WindowsAppRuntime.1.2                           MSIX\\Microsoft.WindowsAppRuntime.1.2_2000.747.1… 2000.747.1945… \r\n" +
                "WindowsAppRuntime.1.2                           MSIX\\Microsoft.WindowsAppRuntime.1.2_2000.747.1… 2000.747.1945… \r\n" +
                "WindowsAppRuntime.1.2                           MSIX\\Microsoft.WindowsAppRuntime.1.2_2000.777.2… 2000.777.2143… \r\n" +
                "WindowsAppRuntime.1.2                           MSIX\\Microsoft.WindowsAppRuntime.1.2_2000.777.2… 2000.777.2143… \r\n" +
                "WindowsAppRuntime.1.2                           MSIX\\Microsoft.WindowsAppRuntime.1.2_2000.802.3… 2000.802.31.0  \r\n" +
                "WindowsAppRuntime.1.2                           MSIX\\Microsoft.WindowsAppRuntime.1.2_2000.802.3… 2000.802.31.0  \r\n" +
                "WindowsAppRuntime.1.3                           MSIX\\Microsoft.WindowsAppRuntime.1.3_3000.851.1… 3000.851.1712… \r\n" +
                "WindowsAppRuntime.1.3                           MSIX\\Microsoft.WindowsAppRuntime.1.3_3000.882.2… 3000.882.2207… \r\n" +
                "WindowsAppRuntime.1.3                           MSIX\\Microsoft.WindowsAppRuntime.1.3_3000.934.1… 3000.934.1904… \r\n" +
                "WindowsAppRuntime.1.3                           MSIX\\Microsoft.WindowsAppRuntime.1.3_3000.934.1… 3000.934.1904… \r\n" +
                "WindowsAppRuntime.1.4                           MSIX\\Microsoft.WindowsAppRuntime.1.4_4000.1049.… 4000.1049.117… \r\n" +
                "WindowsAppRuntime.1.4                           MSIX\\Microsoft.WindowsAppRuntime.1.4_4000.1082.… 4000.1082.225… \r\n" +
                "WindowsAppRuntime.1.4                           MSIX\\Microsoft.WindowsAppRuntime.1.4_4000.1136.… 4000.1136.233… \r\n" +
                "WindowsAppRuntime.1.4                           MSIX\\Microsoft.WindowsAppRuntime.1.4_4000.1309.… 4000.1309.205… \r\n" +
                "WindowsAppRuntime.1.4                           MSIX\\Microsoft.WindowsAppRuntime.1.4_4000.1309.… 4000.1309.205… \r\n" +
                "WindowsAppRuntime.1.5                           MSIX\\Microsoft.WindowsAppRuntime.1.5_5001.159.5… 5001.159.55.0  \r\n" +
                "WindowsAppRuntime.1.5                           MSIX\\Microsoft.WindowsAppRuntime.1.5_5001.178.1… 5001.178.1908… \r\n" +
                "WindowsAppRuntime.1.5                           MSIX\\Microsoft.WindowsAppRuntime.1.5_5001.214.1… 5001.214.1843… \r\n" +
                "WindowsAppRuntime.1.5                           MSIX\\Microsoft.WindowsAppRuntime.1.5_5001.275.5… 5001.275.500.0 \r\n" +
                "WindowsAppRuntime.1.5                           MSIX\\Microsoft.WindowsAppRuntime.1.5_5001.311.2… 5001.311.2039… \r\n" +
                "WindowsAppRuntime.1.5                           MSIX\\Microsoft.WindowsAppRuntime.1.5_5001.373.1… 5001.373.1736… \r\n" +
                "WindowsAppRuntime.1.5                           MSIX\\Microsoft.WindowsAppRuntime.1.5_5001.373.1… 5001.373.1736… \r\n" +
                "WindowsAppRuntime.1.6                           MSIX\\Microsoft.WindowsAppRuntime.1.6_6000.318.2… 6000.318.2304… \r\n" +
                "WindowsAppRuntime.1.6                           MSIX\\Microsoft.WindowsAppRuntime.1.6_6000.373.1… 6000.373.1641… \r\n" +
                "WindowsAppRuntime.1.6                           MSIX\\Microsoft.WindowsAppRuntime.1.6_6000.373.1… 6000.373.1641… \r\n" +
                "Windows-Kamera                                  MSIX\\Microsoft.WindowsCamera_2024.2408.1.0_x64_… 2024.2408.1.0  \r\n" +
                "Feedback-Hub                                    MSIX\\Microsoft.WindowsFeedbackHub_1.2411.17101.… 1.2411.17101.0 \r\n" +
                "Windows-Karten                                  MSIX\\Microsoft.WindowsMaps_11.2411.7.0_x64__8we… 11.2411.7.0    \r\n" +
                "Windows-Editor                                  MSIX\\Microsoft.WindowsNotepad_11.2410.21.0_x64_… 11.2410.21.0   \r\n" +
                "Windows-Audiorekorder                           MSIX\\Microsoft.WindowsSoundRecorder_11.2408.6.0… 11.2408.6.0    \r\n" +
                "Microsoft Store                                 MSIX\\Microsoft.WindowsStore_22412.1401.15.0_x64… 22412.1401.15… \r\n" +
                "Windows-Terminal                                Microsoft.WindowsTerminal                        1.21.3231.0    winget\r\n" +
                "Windows Package Manager Source (winget) V2      MSIX\\Microsoft.Winget.Source_2025.203.1951.32_n… 2025.203.1951… \r\n" +
                "Xbox TCUI                                       MSIX\\Microsoft.Xbox.TCUI_1.24.10001.0_x64__8wek… 1.24.10001.0   \r\n" +
                "Xbox Console Companion                          MSIX\\Microsoft.XboxApp_48.104.4001.0_x64__8weky… 48.104.4001.0  \r\n" +
                "Xbox Zubehör                                    MSIX\\Microsoft.XboxDevices_2411.2411.14001.0_x6… 2411.2411.140… \r\n" +
                "Xbox Game Bar Plugin                            MSIX\\Microsoft.XboxGameOverlay_1.54.4001.0_x64_… 1.54.4001.0    \r\n" +
                "Game Bar                                        MSIX\\Microsoft.XboxGamingOverlay_7.224.11211.0_… 7.224.11211.0  \r\n" +
                "Xbox Identity Provider                          MSIX\\Microsoft.XboxIdentityProvider_12.115.1001… 12.115.1001.0  \r\n" +
                "Game Speech Window                              MSIX\\Microsoft.XboxSpeechToTextOverlay_1.97.170… 1.97.17002.0   \r\n" +
                "Smartphone-Link                                 MSIX\\Microsoft.YourPhone_1.24121.85.0_x64__8wek… 1.24121.85.0   \r\n" +
                "Windows Medienwiedergabe                        MSIX\\Microsoft.ZuneMusic_11.2412.6.0_x64__8weky… 11.2412.6.0    \r\n" +
                "Filme & TV                                      MSIX\\Microsoft.ZuneVideo_10.24111.10061.0_x64__… 10.24111.1006… \r\n" +
                "Remotehilfe                                     MSIX\\MicrosoftCorporationII.QuickAssist_2.0.29.… 2.0.29.0       \r\n" +
                "WinAppRuntime.Main.1.5                          MSIX\\MicrosoftCorporationII.WinAppRuntime.Main.… 5001.373.1736… \r\n" +
                "WinAppRuntime.Singleton                         MSIX\\MicrosoftCorporationII.WinAppRuntime.Singl… 6000.373.1641… \r\n" +
                "Windows-Subsystem für Linux                     Microsoft.WSL                                    2.3.26.0       winget\r\n" +
                "Windows Web Experience Pack                     MSIX\\MicrosoftWindows.Client.WebExperience_524.… 524.34401.20.0 \r\n" +
                "Geräteübergreifender Funktions-Host             MSIX\\MicrosoftWindows.CrossDevice_1.24121.37.0_… 1.24121.37.0   \r\n" +
                "NVIDIA Control Panel                            MSIX\\NVIDIACorp.NVIDIAControlPanel_8.1.967.0_x6… 8.1.967.0      \r\n" +
                "Notepad++                                       MSIX\\NotepadPlusPlus_1.0.0.0_neutral__7njy0v32s… 1.0.0.0        \r\n" +
                "Spotify – Musik und Podcasts                    MSIX\\SpotifyAB.SpotifyMusic_1.256.502.0_x64__zp… 1.256.502.0    \r\n" +
                "Mail und Kalender                               MSIX\\microsoft.windowscommunicationsapps_16005.… 16005.14326.2… \r\n" +
                "Bing Chat                                       MSIX\\www.bing.com-96A23C7C_1.0.0.1_neutral__2y8… 1.0.0.1        \r\n";

            string[] expectedCols = [
                "Name",
                "ID",
                "Version",
                "Quelle"
            ];

            var parser = new WingetParser(wingetOutput);
            Assert.True(parser.HasTable);
            Assert.Equal(expectedCols, parser.Columns);
            Assert.Equal(302, parser.Items.Length);
            Assert.Equal(4, parser.Items[0].Length);
            Assert.Equal("draw.io 26.0.9", parser.Items[0][0]);
            Assert.Equal("JGraph.Draw", parser.Items[0][1]);
            Assert.Equal("26.0.9", parser.Items[0][2]);
            Assert.Equal("winget", parser.Items[0][3]);
        }

        // TODO
        // Constructor_ParseInstalledSomeWithVersion_CheckResult (five column layout)

        [Fact]
        public void Constructor_ParseOutdatedSome_CheckResult()
        {
            // feature:  Output of winget list --upgrade-available with some entries
            // observed: Win 11, winget v1.9.25200

            string wingetOutput = "\r" +
                "   - \r" +
                "   \\ \r" +
                "                                                                                                                        \r" +
                "\r" +
                "   - \r" +
                "   \\ \r" +
                "                                                                                                                        \r" +
                "Name                                              ID                               Version        Verfügbar      Quelle\r\n" +
                "-----------------------------------------------------------------------------------------------------------------------\r\n" +
                "Logitech G HUB                                    Logitech.GHUB                    2024.7.625196  2024.9.9333    winget\r\n" +
                "IntelliJ IDEA Community Edition 2024.3.2.1        JetBrains.IntelliJIDEA.Community 2024.3.2.1     2024.3.2.2     winget\r\n" +
                "MEGAsync                                          Mega.MEGASync                    Unknown        5.7.1.0        winget\r\n" +
                "Microsoft Edge                                    Microsoft.Edge                   132.0.2957.127 132.0.2957.140 winget\r\n" +
                "Ubisoft Connect                                   Ubisoft.Connect                  159.1.11430    159.2.0.11504  winget\r\n" +
                "Microsoft Visual C++ 2013 Redistributable (x86) … Microsoft.VCRedist.2013.x86      12.0.40660.0   12.0.40664.0   winget\r\n" +
                "GOG GALAXY                                        GOG.Galaxy                       Unknown        2.0.80.33      winget\r\n" +
                "EA app                                            ElectronicArts.EADesktop         13.380.0.5893  13.387.0.5900  winget\r\n" +
                "Discord                                           Discord.Discord                  1.0.9147       1.0.9180       winget\r\n" +
                "Dev Home                                          Microsoft.DevHome                0.2000.758.0   0.2001.758.0   winget\r\n" +
                "10 Aktualisierungen verfügbar.\r\n";

            string[] expectedCols = [
                "Name",
                "ID",
                "Version",
                "Verfügbar",
                "Quelle"
            ];

            var parser = new WingetParser(wingetOutput);
            Assert.True(parser.HasTable);
            Assert.Equal(expectedCols, parser.Columns);
            Assert.Equal(10, parser.Items.Length);
            Assert.Equal(5, parser.Items[0].Length);
            Assert.Equal("Logitech G HUB", parser.Items[0][0]);
            Assert.Equal("Logitech.GHUB", parser.Items[0][1]);
            Assert.Equal("2024.7.625196", parser.Items[0][2]);
            Assert.Equal("2024.9.9333", parser.Items[0][3]);
            Assert.Equal("winget", parser.Items[0][4]);
        }

        // TODO
        // Constructor_ParseOutdatedNone_CheckResult
    }
}