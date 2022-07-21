using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using btelnyy.ConfigLoader.API;
using System.Diagnostics;

namespace btelnyy.ConfigLoader.Test
{
    internal static class CommandHandler
    {
        public static void HandleCmd(string[] args)
        {
            string command = args[0];
            try
            {
                switch (command)
                {
                    default:
                        Log.WriteError("Invalid command. Use 'help' for help.");
                        break;
                    case "help":
                        Log.WriteInfo("HELP: \n - loadfile <filepath>: Load a file for later use. (Access with the same filepath) \n - preformancetest <amount> <filepath>: Run a preformance test. Outputs test time after completion. \n - getdata <filepath> <key>: get the value of a specific key in a specific file. \n - setdata <filepath> <key> <value>: Set a value in a file with a key. \n - getdatainfo <filepath> <key>: Get a overview of a key. \n - reloadfile <filename>: Clear config and reload from file.");
                        break;
                    case "preformancetest":
                        PreformanceTest(args);
                        break;
                    case "loadfile":
                        LoadFile(args);
                        break;
                    case "getdata":
                        GetData(args);
                        break;
                    case "setdata":
                        SetData(args);
                        break;
                    case "getdatainfo":
                        GetDataInfo(args);
                        break;
                    case "reloadfile":
                        ReloadFile(args);
                        break;
                }
            }catch(Exception ex)
            {
                Log.WriteError("An error occured while executing your command. \n Message: " + ex.Message + " \n Trace: " + ex.ToString());
            }

        }
        public static void GetDataInfo(string[] args)
        {
            if (args.Length < 3)
            {
                Log.WriteError("Invalid syntax: getdata <filepath> <key>.");
                return;
            }
            Log.WriteLineColor(ConfigManager.GetConfiguration(args[1]).GetDataInfo(args[2]), ConsoleColor.White);
        }
        public static void LoadFile(string[] args)
        {
            ConfigData cd = new();
            if(args.Length < 2)
            {
                Log.WriteError("Invalid syntax: loafile <filepath>.");
                return;
            }
            cd.LoadFile(args[1]);
            ConfigManager.AddConfiguration(cd);
            Log.WriteSuccess("Loaded file.");
        }
        public static void ReloadFile(string[] args)
        {
            ConfigManager.GetConfiguration(args[1]).ReloadFile();
            Log.WriteSuccess("Done.");
        }
        public static void GetData(string[] args)
        {
            if (args.Length < 3)
            {
                Log.WriteError("Invalid syntax: getdata <filepath> <key>.");
                return;
            }
            Log.WriteLineColor("string: " + ConfigManager.GetConfiguration(args[1]).GetString(args[2], "not found"), ConsoleColor.White);
            Log.WriteLineColor("int: " + ConfigManager.GetConfiguration(args[1]).GetInt(args[2], 0).ToString(), ConsoleColor.White);
            Log.WriteLineColor("bool: " + ConfigManager.GetConfiguration(args[1]).GetBool(args[2], false).ToString(), ConsoleColor.White);
        }
        public static void SetData(string[] args)
        {
            if (args.Length < 4)
            {
                Log.WriteError("Invalid syntax: getdata <filepath> <key> <value>.");
                return;
            }
            ConfigManager.GetConfiguration(args[1]).SetString(args[2], args[3]);
            Log.WriteSuccess("Done.");
        }
        public static void PreformanceTest(string[] args)
        {
            Dictionary<string, string> testdict1 = new();
            ConfigData cd = new();
            int RunFor = 10;
            string path = @".\test_config.txt";
            if (args.Length < 2)
            {
                Log.WriteWarning("This command takes 3 arguments: amounttest <amount> <configpath>, its ok as I am using backup arguments.");
            }
            else
            {
                path = args[2];
                RunFor = Convert.ToInt32(args[1]);
            }
            Log.WriteDebug("Running creating and loading " + RunFor.ToString() + " entries.");
            Stopwatch sw = new();
            for (int i = 0; i < RunFor; i++)
            {
                Log.WriteDebug("[TEST] Adding entry to dict: " + i.ToString());
                testdict1.Add("key_" + i.ToString(), "value_" + i.ToString());
            }
            sw.Stop();
            cd.CreateConfigFile(path, testdict1);
            Log.WriteDebug("[TEST] Took " + sw.ElapsedMilliseconds.ToString() + "ms to add " + RunFor.ToString() + " entries to the dictionary.");
        }
    }
}
