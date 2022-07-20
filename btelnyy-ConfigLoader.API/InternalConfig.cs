using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btelnyy.ConfigLoader.API
{
    internal class InternalConfig
    {
        public static string? LogPath = @".\";
        public static bool EnableLogging = false;
        public static bool ShowLogsInConsole = true;
        public static void HandleConfig(InternalConfigOptions option, object value)
        {
            switch (option)
            {
                case InternalConfigOptions.LOG_PATH:
                    if(Convert.ToString(value) == null)
                    {
                        LogPath = @".\";
                    }
                    else
                    {
                        LogPath = Convert.ToString(value);
                    }
                    break;
                case InternalConfigOptions.ENABLE_LOGGING:
                    EnableLogging = Convert.ToBoolean(value);
                    break;
                case InternalConfigOptions.SHOW_LOGS_IN_CONSOLE:
                    ShowLogsInConsole = Convert.ToBoolean(value);
                    break;
            }
        }
    }
    public enum InternalConfigOptions
    {
        LOG_PATH,
        ENABLE_LOGGING,
        SHOW_LOGS_IN_CONSOLE,
    }
}
