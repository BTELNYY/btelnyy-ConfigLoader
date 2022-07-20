﻿using System;
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
                Entries.Add(Utility.LineBuilder(key, dict[key]));
            }
            File.WriteAllLines(filePath, Entries);
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
        public void CreateConfigFile(string filePath, Dictionary<string, Config> dict)
        {
            LinkedList<string> Entries = new();
            foreach (string key in dict.Keys)
            {
                Entries.AddLast(Utility.TagsLineBuilder(dict[key].DataTags));
                Entries.AddLast(Utility.LineBuilder(key, dict[key].Data));
            }
            File.WriteAllLines(filePath, Entries);
            LoadFile(filePath);
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
                    //make sure comments still count as a line, so they wont get overwritten
                    LineCounter++;
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