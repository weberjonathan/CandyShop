using System;
using System.Collections.Generic;
using System.Text;

namespace CandyShop.Controller
{
    interface IMainWindowController
    {
        void SmartSelectPackages();
        void ShowGithub();
        void ShowMetaPackageHelp();
        void ShowLicenses();
        void ToggleCreateTask();
    }
}
