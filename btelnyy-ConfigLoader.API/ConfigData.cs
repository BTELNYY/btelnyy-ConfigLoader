using System;
using System.Linq;
using System.Security;
using System.Diagnostics;

namespace btelnyy.ConfigLoader.API
{
    public class ConfigData
    {
        readonly Dictionary<string, ConfigEntry> Configs = new();
        string FilePath = "";
        string Seperator = ":";

        #region Getters and Setters.
        /// <summary>
        /// Get the config file sperator, this controls key value pairs. e.g. key:value or key=value
        /// </summary>
        /// <returns>
        /// The string used to seperate the key value pairs
        /// </returns>
        public string GetSeperator()
        {
            return Seperator;
        }
        /// <summary>
        /// Set the config file sperator, this controls key value pairs. e.g. key:value or key=value
        /// </summary>
        /// <param name="value">
        /// The Seperator to set for the config value.
        /// </param>
        public void SetSeperator(string value)
        {
            Seperator = value;
        }
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
        /// Get the raw ConfigEntry type of a key
        /// </summary>
        /// <param name="key">
        /// Key which to get data for
        /// </param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// Thrown if the key does not exist.
        /// </exception>
        public ConfigEntry GetRawEntry(string key)
        {
            if (!Configs.ContainsKey(key))
            {
                throw new ArgumentException("Key provided does not exist within the current configuration.", new NullReferenceException());
            }
            return Configs[key];
        }
        /// <summary>
        /// Returns a overview of the key/value pair as a string.
        /// </summary>
        /// <param name="key">
        /// Key which to show data for.
        /// </param>
        /// <returns></returns>
        public string GetDataInfo(string key)
        {
            return Configs[key].ToString();
        }
        /// <summary>
        /// Get all keys of the current config file.
        /// </summary>
        /// <returns>
        /// A IEnumerable<string> which contains all the keys of the config file. Any keys with the [hidden] tag are not shown.
        /// </returns>
        public IEnumerable<string> GetKeys()
        {
            List<string> keys = new();
            foreach(string key in Configs.Keys)
            {
                if (Configs[key].DataTags.Contains(Tags.HIDDEN))
                {
                    continue;
                }
                keys.Add(key);
            }
            return keys;
        }
        /// <summary>
        /// Get the count of entires in the config file.
        /// </summary>
        /// <returns>
        /// An int32 amount of configuration entries.
        /// </returns>
        public int GetConfigLength()
        {
            return Configs.Count;
        }
        #endregion

        #region Main Methods: Loading, Reloading, Creating files.
        /// <summary>
        /// Clear the dictionary and reload the configs from file.
        /// </summary>
        public void ReloadFile()
        {
            Log.WriteInfo("Database original length: " + Configs.Count);
            Configs.Clear();
            Log.WriteInfo("Cleared Config database. File: " + FilePath);
            LoadFile(FilePath);
            Log.WriteInfo("New database length: " + Configs.Count);
        }
        /// <summary>
        /// Create and Load a new config file from a dictionary. This  method does not add any Tags to the created configuration file
        /// </summary>
        /// <param name="filePath">
        /// Path of the file you wish to create/write to
        /// </param>
        /// <param name="dict">
        /// Dictianory of string, string for key value pairs.
        /// </param>
        public void CreateConfigFile(string filePath, Dictionary<string, string> dict)
        {
            List<string> Entries = new();
            foreach(string key in dict.Keys)
            {
                Entries.Add(Utility.LineBuilder(key, dict[key], Seperator));
            }
            File.WriteAllLines(filePath, Entries);
            Log.WriteInfo("Created new file: " + filePath + " Entries: " + dict.Count.ToString());
            LoadFile(filePath);
        }
        /// <summary>
        /// Create a new config file with pre-existing Config objects in a dictionary. Automatically loads file after creation.
        /// </summary>
        /// <param name="filePath">
        /// File which to create.
        /// </param>
        /// <param name="dict">
        /// Dictionary which will be inputed into the file.
        /// </param>
        public void CreateConfigFile(string filePath, Dictionary<string, ConfigEntry> dict)
        {
            LinkedList<string> Entries = new();
            foreach (string key in dict.Keys)
            {
                Entries.AddLast(Utility.TagsLineBuilder(dict[key].DataTags));
                Entries.AddLast(Utility.LineBuilder(key, dict[key].Data, Seperator));
            }
            File.WriteAllLines(filePath, Entries);
            Log.WriteInfo("Created new file: " + filePath + " Entries: " + dict.Count.ToString());
            LoadFile(filePath);
        }
        /// <summary>
        /// Loads a file based on the path from param Path into its own object
        /// </summary>
        /// <param name="Path"></param>
        public void LoadFile(string Path)
        {
            Stopwatch sw = new();
            sw.Start();
            Log.WriteInfo("Loading file " + Path + "!");
            FilePath = Path;
            //read all text from the file
            string FileData = File.ReadAllText(Path);
            //split by newline
            string[] Data = FileData.Split('\n');
            ConfigEntry config = new();
            //for tags and such
            bool DoNotCreateNewEntry = false;
            int LineCounter = 0;
            foreach (string DataEntry in Data)
            {
                Log.WriteDebug("Exact Data Entry contender: " + DataEntry);
                //exclude comments
                if (DataEntry.StartsWith('#'))
                {
                    Log.WriteInfo("Comment on line " + LineCounter.ToString() + " ignored.");
                    //make sure comments still count as a line, so they wont get overwritten
                    LineCounter++;
                    continue;
                }
                if(string.IsNullOrWhiteSpace(DataEntry) || string.IsNullOrEmpty(DataEntry)) 
                {

                    Log.WriteInfo("Blank entry on line " + LineCounter.ToString() + " ignored.");
                    //make sure comments still count as a line, so they wont get overwritten
                    LineCounter++;
                    continue;
                }
                if (!DoNotCreateNewEntry)
                {
                    Log.WriteInfo("Creating new entry! Line: " + LineCounter.ToString());
                    config = new();
                }
                //check for tags
                if (DataEntry.StartsWith('!')) 
                {
                    Log.WriteInfo("Line " + LineCounter.ToString() + " is a tag, calculating now.");
                    DoNotCreateNewEntry = true;
                    string DataTags = DataEntry;
                    DataTags = DataTags.ToUpper();
                    DataTags = DataTags.Trim('!');
                    string[] DataTagsArray = DataTags.Split(',');
                    foreach(string DataTag in DataTagsArray)
                    {
                        try
                        {
                            Log.WriteInfo("Parsing Tag: " + DataTag);
                            Tags tag = Enum.Parse<Tags>(DataTag);
                            config.DataTags.Add(tag);
                        }
                        catch(ArgumentException ex)
                        {
                            Log.WriteError("Error occured while parsing tag: \n" + ex.ToString());
                            continue;
                        }
                    }
                }
                else //no tags
                {
                    Log.WriteInfo("No tags on line " + LineCounter.ToString() + " detected.");
                    DoNotCreateNewEntry = false;
                }
                //get key and value
                string[] DataEntryParts = DataEntry.Split(Seperator);
                string Key = DataEntryParts.First();
                string Value = "";
                //if the value contains ":"
                if(DataEntryParts.Length > 2)
                {
                    Log.WriteInfo("Multiple colons detected, skipping first to create value. Line: " + LineCounter.ToString());
                    string[] DataValues = DataEntryParts.Skip(1).ToArray();
                    Value = string.Concat(DataValues);
                }
                Value = DataEntryParts.Last();
                config.Data = Value;
                config.OriginLine = LineCounter;
                //add extra properties
                if(config.DataTags.Count > 0)
                {
                    Log.WriteInfo("Line " + LineCounter.ToString() + " has tags attached. Tag count: " + config.DataTags.Count.ToString());
                }
                if (config.DataTags.Contains(Tags.READONLY))
                {
                    Log.WriteInfo("Line " + LineCounter.ToString() + " is considered readonly, setting flag.");
                    config.Readonly = true;
                }
                Configs.Add(Key, config);
                LineCounter++;
            }
            sw.Stop();
            Log.WriteDebug("Loading file with length of " + Configs.Count.ToString() + " took " + sw.ElapsedMilliseconds + "ms");
            float TimePerEntry = (float) Configs.Count / (float) sw.ElapsedMilliseconds;
            int ShownDigits = 2;
            Log.WriteDebug("Took " + Math.Round(TimePerEntry, ShownDigits).ToString() + "ms per entry");
        }
        #endregion

        #region Data Manipulation: Getting Data, Setting Data, Adding and Removing Tags
        /// <summary>
        /// Same as AddTags, except only adds one tag.
        /// </summary>
        /// <param name="key">
        /// Key to apply to
        /// </param>
        /// <param name="tag">
        /// Tag to add
        /// </param>
        /// <exception cref="ArgumentException">
        /// Key is not found
        /// </exception>
        /// <exception cref="SecurityException">
        /// Tags are write-protected
        /// </exception>
        public void AddTag(string key, Tags tag)
        {
            Tags[] tags = { tag }; 
            if (!Configs.ContainsKey(key))
            {
                throw new ArgumentException("Key " + key + " is not found, load it with GetString() before adding tags.");
            }
            if (Configs[key].DataTags.Contains(Tags.LOCK_TAGS))
            {
                throw new SecurityException("Key " + key + "has tags locked! Tags cannot be modified for this key.");
            }
            List<string> lines = File.ReadAllLines(FilePath).ToList();
            int configLine = Configs[key].OriginLine;
            lines.Insert(configLine - 1, Utility.TagsLineBuilder(tags));
            File.WriteAllLines(FilePath, lines.ToArray());
        }
        /// <summary>
        /// Same as RemoveTags, except only removes one tag.
        /// </summary>
        /// <param name="key">
        /// Key to apply to
        /// </param>
        /// <param name="tag">
        /// Tag to remove
        /// </param>
        /// <exception cref="ArgumentException">
        /// Key is not found
        /// </exception>
        /// <exception cref="SecurityException">
        /// Tags are write-protected
        /// </exception>
        public void RemoveTag(string key, Tags tag)
        {
            Tags[] tags = { tag };
            if (!Configs.ContainsKey(key))
            {
                throw new ArgumentException("Key " + key + " is not found, load it with GetString() before adding tags.");
            }
            if (Configs[key].DataTags.Contains(Tags.LOCK_TAGS))
            {
                throw new SecurityException("Key " + key + "has tags locked! Tags cannot be modified for this key.");
            }
            foreach (Tags t in tags)
            {
                if (Configs[key].DataTags.Contains(t))
                {
                    Configs[key].DataTags.Remove(t);
                }
            }
            List<string> lines = File.ReadAllLines(FilePath).ToList();
            int configLine = Configs[key].OriginLine;
            lines.Insert(configLine - 1, Utility.TagsLineBuilder(Configs[key].DataTags));
            File.WriteAllLines(FilePath, lines.ToArray());
        }
        /// <summary>
        /// Add tags to an entry.
        /// </summary>
        /// <param name="key">
        /// Entry key.
        /// </param>
        /// <param name="tags">
        /// IEnumerable of tags to add to the entry.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Key does not exist in Config.
        /// </exception>
        /// <exception cref="SecurityException">
        /// Tags are locked by the config.
        /// </exception>
        public void AddTags(string key, IEnumerable<Tags> tags)
        {
            if (!Configs.ContainsKey(key))
            {
                throw new ArgumentException("Key " + key + " is not found, load it with GetString() before adding tags.");
            }
            if (Configs[key].DataTags.Contains(Tags.LOCK_TAGS))
            {
                throw new SecurityException("Key " + key + "has tags locked! Tags cannot be modified for this key.");
            }
            List<string> lines = File.ReadAllLines(FilePath).ToList();
            int configLine = Configs[key].OriginLine;
            lines.Insert(configLine - 1, Utility.TagsLineBuilder(tags));
            File.WriteAllLines(FilePath, lines.ToArray());
        }
        /// <summary>
        /// Remove tags from a entry.
        /// </summary>
        /// <param name="key">
        /// Key of the entry.
        /// </param>
        /// <param name="tags">
        /// IEnumerable of Tags to remove.
        /// </param>
        /// <exception cref="ArgumentException">
        /// Key is not found.
        /// </exception>
        /// <exception cref="SecurityException">
        /// Tags are locked.
        /// </exception>
        public void RemoveTags(string key, IEnumerable<Tags> tags)
        {
            if (!Configs.ContainsKey(key))
            {
                throw new ArgumentException("Key " + key + " is not found, load it with GetString() before adding tags.");
            }
            if (Configs[key].DataTags.Contains(Tags.LOCK_TAGS))
            {
                throw new SecurityException("Key " + key + "has tags locked! Tags cannot be modified for this key.");
            }
            foreach(Tags tag in tags)
            {
                if (Configs[key].DataTags.Contains(tag))
                {
                    Configs[key].DataTags.Remove(tag);
                }
            }
            List<string> lines = File.ReadAllLines(FilePath).ToList();
            int configLine = Configs[key].OriginLine;
            lines.Insert(configLine - 1, Utility.TagsLineBuilder(Configs[key].DataTags));
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
                ConfigEntry cfg = new()
                {
                    Data = value
                };
                Configs.Add(key, cfg);
                return;
            }
            //if readonly
            if (Configs[key].Readonly)
            {
                throw new SecurityException("Value " + key + " is readonly!");
            }
            if (Configs[key].DataTags.Contains(Tags.NOTIFY_ON_CHANGE))
            {
                Log.WriteInfo("Value of " + key + " has changed! This message is being sent becuase the key has the NOTIFY_ON_CHANGE tag");
            }
            Configs[key].Data = value;
            Utility.LineChange(FilePath, Configs[key].OriginLine, Utility.LineBuilder(key, value, Seperator));
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
                ConfigEntry cfg = new()
                {
                    Data = value.ToString()
                };
                Configs.Add(key, cfg);
                return;
            }
            //if readonly
            if (Configs[key].Readonly)
            {
                throw new SecurityException("Value " + key + " is readonly!");
            }
            Configs[key].Data = value.ToString();
            Utility.LineChange(FilePath, Configs[key].OriginLine, Utility.LineBuilder(key, value.ToString(), Seperator));
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
                ConfigEntry cfg = new()
                {
                    Data = value.ToString()
                };
                Configs.Add(key, cfg);
                return;
            }
            //if readonly
            if (Configs[key].Readonly)
            {
                throw new SecurityException("Value " + key + " is readonly!");
            }
            Configs[key].Data = value.ToString();
            Utility.LineChange(FilePath, Configs[key].OriginLine, Utility.LineBuilder(key, value.ToString(), Seperator));
        }
        /// <summary>
        /// Load a key without actually returning it
        /// </summary>
        /// <param name="key">
        /// The key which to load
        /// </param>
        /// <param name="defaultvalue">
        /// The default value in which to put if the key is not found
        /// </param>
        public void LoadKey(string key, string defaultvalue) 
        {
            if (!Configs.ContainsKey(key))
            {
                ConfigEntry cfg = new()
                {
                    Data = defaultvalue,
                    OriginLine = (Utility.GetLength(FilePath) + 1)
                };
                Utility.LineChange(FilePath, (Utility.GetLength(FilePath) + 1), Utility.LineBuilder(key, defaultvalue.ToString(), Seperator));
                Configs.Add(key, cfg);
            }
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
                ConfigEntry cfg = new()
                {
                    Data = defaultvalue,
                    OriginLine = (Utility.GetLength(FilePath) + 1)
                };
                Utility.LineChange(FilePath, (Utility.GetLength(FilePath) + 1), Utility.LineBuilder(key, defaultvalue.ToString(), Seperator));
                Configs.Add(key, cfg);
            }
            return Configs[key].Data;
        }
        /// <summary>
        /// Get a unsigned int from config
        /// </summary>
        /// <param name="key">
        /// Key name
        /// </param>
        /// <param name="defaultvalue">
        /// Default value to write if value not found
        /// </param>
        /// <returns>
        /// uint value of the key
        /// </returns>
        public uint GetUInt(string key, uint defaultvalue)
        {
            if (!Configs.ContainsKey(key))
            {
                ConfigEntry cfg = new()
                {
                    Data = defaultvalue.ToString()
                };
                Configs.Add(key, cfg);
                cfg.OriginLine = (Utility.GetLength(FilePath) + 1);
                Utility.LineChange(FilePath, (Utility.GetLength(FilePath) + 1), Utility.LineBuilder(key, defaultvalue.ToString(), Seperator));
            }
            try
            {
                return Convert.ToUInt32(Configs[key].Data);
            }
            catch (Exception ex)
            {
                if (InternalConfig.ShowConversionWarnings)
                {
                    Log.WriteWarning("Error occured parsing value " + Configs[key].Data + "\n" + ex.ToString());
                }
                string datatoparse = Configs[key].Data;
                if (uint.TryParse(datatoparse, out uint parsed))
                {
                    return parsed;
                }
                else
                {
                    return defaultvalue;
                }
            }
        }
        /// <summary>
        /// Get a unsigned long from config
        /// </summary>
        /// <param name="key">
        /// Key name
        /// </param>
        /// <param name="defaultvalue">
        /// Default value to write if value not found
        /// </param>
        /// <returns>
        /// ulong value of the key
        /// </returns>
        public ulong GetULong(string key, uint defaultvalue)
        {
            if (!Configs.ContainsKey(key))
            {
                ConfigEntry cfg = new()
                {
                    Data = defaultvalue.ToString()
                };
                Configs.Add(key, cfg);
                cfg.OriginLine = (Utility.GetLength(FilePath) + 1);
                Utility.LineChange(FilePath, (Utility.GetLength(FilePath) + 1), Utility.LineBuilder(key, defaultvalue.ToString(), Seperator));
            }
            try
            {
                return Convert.ToUInt64(Configs[key].Data);
            }
            catch (Exception ex)
            {
                if (InternalConfig.ShowConversionWarnings)
                {
                    Log.WriteWarning("Error occured parsing value " + Configs[key].Data + "\n" + ex.ToString());
                }
                string datatoparse = Configs[key].Data;
                if (ulong.TryParse(datatoparse, out ulong parsed))
                {
                    return parsed;
                }
                else
                {
                    return defaultvalue;
                }
            }
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
                ConfigEntry cfg = new()
                {
                    Data = defaultvalue.ToString()
                };
                Configs.Add(key, cfg);
                cfg.OriginLine = (Utility.GetLength(FilePath) + 1);
                Utility.LineChange(FilePath, (Utility.GetLength(FilePath) + 1), Utility.LineBuilder(key, defaultvalue.ToString(), Seperator));
            }
            try
            {
                return Convert.ToInt32(Configs[key].Data);
            }
            catch(Exception ex)
            {
                if (InternalConfig.ShowConversionWarnings)
                {
                    Log.WriteWarning("Error occured parsing value " + Configs[key].Data + "\n" + ex.ToString());
                }
                //neat trick to get around parsing bools
                string datatoparse = Configs[key].Data;
                if (int.TryParse(datatoparse, out int parsed))
                {
                    return parsed;
                }
                else
                {
                    return defaultvalue;
                }
            }
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
                ConfigEntry cfg = new()
                {
                    Data = defaultvalue.ToString()
                };
                Configs.Add(key, cfg);
                cfg.OriginLine = (Utility.GetLength(FilePath) + 1);
                Utility.LineChange(FilePath, (Utility.GetLength(FilePath) + 1), Utility.LineBuilder(key, defaultvalue.ToString(), Seperator));
            }
            try
            {
                return Convert.ToBoolean(Configs[key].Data.ToLower());
            }
            catch(Exception ex)
            {
                if (InternalConfig.ShowConversionWarnings)
                {
                    Log.WriteWarning("Error occured parsing value " + Configs[key].Data + "\n" + ex.ToString());
                }
                string datatoparse = Configs[key].Data;
                if (bool.TryParse(datatoparse, out bool parsed))
                {
                    return parsed;
                }
                else
                {
                    return defaultvalue;
                }
            }
        }
        
        public List<string> GetList(string key, List<string> defaultvalue)
        {
            if (!Configs.ContainsKey(key))
            {
                SetList(key, defaultvalue);
                return defaultvalue;
            }
            else
            {
                return Utility.ListParse(Configs[key].Data);
            }
        }

        public void SetList(string key, List<string> value)
        {
            if (Configs.ContainsKey(key))
            {
                Log.WriteWarning("Key already exists.");
            }
            else
            {
                ConfigEntry entry = new ConfigEntry
                {
                    Data = Utility.ListParse(value),
                    OriginLine = Utility.GetLength(FilePath) + 1
                };
                Configs.Add(key, entry);
                Utility.LineChange(FilePath, (Utility.GetLength(FilePath) + 1), Utility.LineBuilder(key, Utility.ListParse(value), Seperator));
            }
        }

        public Dictionary<string, string> GetDict(string key, Dictionary<string, string> defaultvalue)
        {
            if (!Configs.ContainsKey(key))
            {
                SetDict(key, defaultvalue);
                return defaultvalue;
            }
            else
            {
                return Utility.DictParse(Configs[key].Data);
            }
        }

        public void SetDict(string key, Dictionary<string, string> value)
        {
            if (Configs.ContainsKey(key))
            {
                Log.WriteWarning("Key already exists.");
            }
            else
            {
                ConfigEntry entry = new ConfigEntry
                {
                    Data = Utility.DictParse(value),
                    OriginLine = Utility.GetLength(FilePath) + 1
                };
                Configs.Add(key, entry);
                Utility.LineChange(FilePath, (Utility.GetLength(FilePath) + 1), Utility.LineBuilder(key, Utility.DictParse(value), Seperator));
            }
        }

        #endregion
    }
}