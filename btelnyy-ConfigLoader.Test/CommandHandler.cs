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
                        Log.WriteInfo("HELP: \n - loadfile <filepath>: Load a file for later use. (Access with the same filepath) \n - amounttest <amount> <filepath>: run a quick test for preformance with large or small amounts of data.");
                        break;
                    case "amounttest":
                        AmountTest(args);
                        break;
                    case "loadfile":
                        LoadFile(args);
                        break;
                }
            }catch(Exception ex)
            {
                Log.WriteError("An error occured while executing your command. \n Message: " + ex.Message + " \n Trace: " + ex.ToString());
            }

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
        public static void AmountTest(string[] args)
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
