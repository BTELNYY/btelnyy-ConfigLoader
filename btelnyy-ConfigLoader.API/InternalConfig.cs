using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btelnyy.ConfigLoader.API
{
    public class InternalConfig
    {
        //fuck you dumbass warnings!
#pragma warning disable CA2211 // Non-constant fields should not be visible
        public static string? LogPath = @".\";
        public static bool EnableLogging = false;
        public static bool ShowLogsInConsole = true;
        public static bool ShowConversionWarnings = false;
#pragma warning restore CA2211 // Non-constant fields should not be visible
        public static readonly string Version = "1.0.0";
    }
}
