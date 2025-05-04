using CandyShop.Properties;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows.Forms;

namespace CandyShop
{
    /// <summary>
    /// Determines and contains relevant information for the execution of CandyShop, such as command-line options and settings
    /// </summary>
    internal class Arguments
    {
        private const string OPTION_BACKGROUND = "--silent";
        private const string OPTION_BACKGROUND_SHORT = "-s";
        private const string OPTION_BACKGROUND_LEGACY = "--background";
        private const string OPTION_BACKGROUND_LEGACY_SHORT = "-b";
        private const string OPTION_DEBUG = "--debug";
        private const string OPTION_ENABLE_SELF_UPDATE = "--enable-self-update";


        public Arguments()
        {
            ParseArguments();
        }

        // ----------------- set through arguments -----------------

        public bool LaunchedMinimized { get; set; } = false;

        public bool DebugEnabled { get; private set; } = false;

        public bool SelfUpdateEnabled { get; private set; } = false;

        // ---------------------------------------------------------

        private void ParseArguments()
        {
            Queue<string> arguments = new Queue<string>(Environment.GetCommandLineArgs());
            while (arguments.Count > 0)
            {
                string arg = arguments.Dequeue();
                switch (arg)
                {
                    case OPTION_BACKGROUND:
                        LaunchedMinimized = true;
                        break;
                    case OPTION_BACKGROUND_SHORT:
                        LaunchedMinimized = true;
                        break;
                    case OPTION_BACKGROUND_LEGACY:
                        LaunchedMinimized = true;
                        break;
                    case OPTION_BACKGROUND_LEGACY_SHORT:
                        LaunchedMinimized = true;
                        break;
                    case OPTION_DEBUG:
                        DebugEnabled = true;
                        break;
                    case OPTION_ENABLE_SELF_UPDATE:
                        SelfUpdateEnabled = true;
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
