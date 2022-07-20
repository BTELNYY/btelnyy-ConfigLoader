using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace btelnyy.ConfigLoader.API
{
    public class ConfigManager
    {
        static readonly Dictionary<string, ConfigLoader> ConfigLoaders = new();
        /// <summary>
        /// Get existing configuration from the in built dictionary
        /// </summary>
        /// <param name="FilePath">
        /// Path to the file of which you want to load
        /// </param>
        /// <returns>
        /// Instance of the ConfigLoader Object
        /// </returns>
        public static ConfigLoader GetConfiguration(string FilePath)
        {
            if (ConfigLoaders.ContainsKey(FilePath))
            {
                return ConfigLoaders[FilePath];
            }
            ConfigLoader cl = new();
            cl.LoadFile(FilePath);
            ConfigLoaders.Add(FilePath, cl);
            return cl;
        }
        /// <summary>
        /// Load a new configuration from a file path
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns>
        /// Instance of the ConfigLoader class
        /// </returns>
        public static ConfigLoader LoadConfiguration(string FilePath)
        {
            if (ConfigLoaders.ContainsKey(FilePath))
            {
                return ConfigLoaders[FilePath];
            }
            ConfigLoader cl = new();
            cl.LoadFile(FilePath);
            ConfigLoaders.Add(FilePath, cl);
            return cl;
        }
        /// <summary>
        /// Add a Config to the dictionary
        /// </summary>
        /// <param name="ConfigLoader">
        /// Instance of the ConfigLoader class which you wish to add as that files configuration
        /// </param>
        public static void AddConfiguration(ConfigLoader ConfigLoader)
        {
            string FilePath = ConfigLoader.GetFilePath();
            if (ConfigLoaders.ContainsKey(FilePath))
            {
                return;
            }
            ConfigLoaders.Add(FilePath, ConfigLoader);
        }
        /// <summary>
        /// Replace a existing entry with a new one
        /// </summary>
        /// <param name="ConfigLoader">
        /// Instance of the ConfigLoader class
        /// </param>
        public static void EditConfiguration(ConfigLoader ConfigLoader)
        {
            string FilePath = ConfigLoader.GetFilePath();
            if (!ConfigLoaders.ContainsKey(FilePath))
            {
                ConfigLoaders.Add(FilePath, ConfigLoader);
                return;
            }
        }
    }
}
