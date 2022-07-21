using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btelnyy.ConfigLoader.API
{
    public class InternalConfig
    {
        public static string? LogPath = @".\";
        public static bool EnableLogging = false;
        public static bool ShowLogsInConsole = true;
        public static bool ShowConversionWarnings = false;
    }
}
