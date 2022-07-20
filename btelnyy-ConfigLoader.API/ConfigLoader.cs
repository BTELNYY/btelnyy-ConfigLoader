using System;
using System.Linq;
using System.Security;

namespace btelnyy.ConfigLoader.API
{
    public class ConfigLoader
    {
        readonly Dictionary<string, Config> Configs = new();
        string FilePath = "";
        /// <summary>
        /// Get the file path of the current configuration object
        /// </summary>
        /// <returns>
        /// returns a <see cref="string"/> of the filepath
        /// </returns>
        public string GetFilePath()
        {
            return FilePath;
        }
        /// <summary>
        /// Loads a file based on the path from param Path into its own object
        /// </summary>
        /// <param name="Path"></param>
        public void LoadFile(string Path)
        {
            FilePath = Path;
            //read all text from the file
            string FileData = File.ReadAllText(Path);
            //split by newline
            string[] Data = FileData.Split('\n');
            Config config = new();
            //for tags and such
            bool DoNotCreateNewEntry = false;
            int LineCounter = 0;
            foreach (string DataEntry in Data)
            {
                //exclude comments
                if (DataEntry.StartsWith('#'))
                {
                    continue;
                }
                if (!DoNotCreateNewEntry)
                {
                    config = new();
                }
                //check for tags
                if (DataEntry.StartsWith('[') && DataEntry.EndsWith(']')) 
                {
                    DoNotCreateNewEntry = true;
                    string DataTags = DataEntry.Trim('[', ']');
                    DataTags += DataTags.ToUpper();
                    string[] DataTagsArray = DataTags.Split(',');
                    foreach(string DataTag in DataTagsArray)
                    {
                        try
                        {
                            Tags tag = Enum.Parse<Tags>(DataTag);
                            config.DataTags.Add(tag);
                        }
                        catch(ArgumentException ex)
                        {
                            ex.ToString();
                            continue;
                        }
                    }
                }
                else //no tags
                {
                    DoNotCreateNewEntry = false;
                }
                //get key and value
                string[] DataEntryParts = DataEntry.Split(":");
                string Key = DataEntryParts.First();
                string Value = "";
                //if the value contains ":"
                if(DataEntryParts.Length > 2)
                {
                    string[] DataValues = DataEntryParts.Skip(1).ToArray();
                    Value = string.Concat(DataValues);
                }
                Value = DataEntryParts.Last();
                config.Data = Value;
                config.OriginLine = LineCounter;
                //add extra properties
                if (config.DataTags.Contains(Tags.READONLY))
                {
                    config.Readonly = true;
                }
                Configs.Add(Key, config);
                LineCounter++;
            }
        }
        public void AddTags(string key, IEnumerable<Tags> tags)
        {
            if (!Configs.ContainsKey(key))
            {
                throw new ArgumentException("Key " + key + " is not found, load it with GetString() before adding tags.");
            }
            List<string> lines = File.ReadAllLines(FilePath).ToList();
            int configLine = Configs[key].OriginLine;
            lines.Insert(configLine - 1, Utility.TagsLineBuilder(tags));
            File.WriteAllLines(FilePath, lines.ToArray());
        }
        /// <summary>
        /// Sets a value based on key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="SecurityException">
        /// Throws if the value is readonly
        /// </exception>
        public void SetString(string key, string value)
        {
            if (!Configs.ContainsKey(key))
            {
                Config cfg = new();
                cfg.Data = value;
                Configs.Add(key, cfg);
                return;
            }
            //if readonly
            if (Configs[key].Readonly)
            {
                throw new SecurityException("Value " + key + " is readonly!");
            }
            Configs[key].Data = value;
            Utility.LineChange(FilePath, Configs[key].OriginLine, Utility.LineBuilder(key, value));
        }
        /// <summary>
        /// Sets a value based on key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="SecurityException">
        /// Throws if the value is readonly
        /// </exception>
        public void SetInt(string key, int value)
        {
            if (!Configs.ContainsKey(key))
            {
                Config cfg = new();
                cfg.Data = value.ToString();
                Configs.Add(key, cfg);
                return;
            }
            //if readonly
            if (Configs[key].Readonly)
            {
                throw new SecurityException("Value " + key + " is readonly!");
            }
            Configs[key].Data = value.ToString();
            Utility.LineChange(FilePath, Configs[key].OriginLine, Utility.LineBuilder(key, value.ToString()));
        }
        /// <summary>
        /// Sets a value based on key and value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <exception cref="SecurityException">
        /// Throws if the value is readonly
        /// </exception>
        public void SetBool(string key, bool value)
        {
            if (!Configs.ContainsKey(key))
            {
                Config cfg = new();
                cfg.Data = value.ToString();
                Configs.Add(key, cfg);
                return;
            }
            //if readonly
            if (Configs[key].Readonly)
            {
                throw new SecurityException("Value " + key + " is readonly!");
            }
            Configs[key].Data = value.ToString();
            Utility.LineChange(FilePath, Configs[key].OriginLine, Utility.LineBuilder(key, value.ToString()));
        }
        /// <summary>
        /// Gets a value based on the input parameters
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultvalue">
        /// If the key is not found, this will be returned and set in the file.
        /// </param>
        /// <returns></returns>
        public string GetString(string key, string defaultvalue)
        {
            if (!Configs.ContainsKey(key))
            {
                Config cfg = new();
                cfg.Data = defaultvalue;
                cfg.OriginLine = (Utility.GetLength(FilePath) + 1);
                Utility.LineChange(FilePath, (Utility.GetLength(FilePath) + 1), Utility.LineBuilder(key, defaultvalue.ToString()));
                Configs.Add(key, cfg);
            }
            return Configs[key].Data;
        }
        /// <summary>
        /// Gets a value based on the input parameters
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultvalue">
        /// If the key is not found, this will be returned and set in the file.
        /// </param>
        /// <returns></returns>
        public int GetInt(string key, int defaultvalue)
        {
            if (!Configs.ContainsKey(key))
            {
                Config cfg = new();
                cfg.Data = defaultvalue.ToString();
                Configs.Add(key, cfg);
                cfg.OriginLine = (Utility.GetLength(FilePath) + 1);
                Utility.LineChange(FilePath, (Utility.GetLength(FilePath) + 1), Utility.LineBuilder(key, defaultvalue.ToString()));
            }
            return Convert.ToInt32(Configs[key].Data);
        }
        /// <summary>
        /// Gets a value based on the input parameters
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultvalue">
        /// If the key is not found, this will be returned and set in the file.
        /// </param>
        /// <returns></returns>
        public bool GetBool(string key, bool defaultvalue)
        {
            if (!Configs.ContainsKey(key))
            {
                Config cfg = new();
                cfg.Data = defaultvalue.ToString();
                Configs.Add(key, cfg);
                cfg.OriginLine = (Utility.GetLength(FilePath) + 1);
                Utility.LineChange(FilePath, (Utility.GetLength(FilePath) + 1), Utility.LineBuilder(key, defaultvalue.ToString()));
            }
            return Convert.ToBoolean(Configs[key].Data);
        }
    }
}