using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using btelnyy.ConfigLoader.API;

namespace btelnyy.ConfigLoader.Test
{
    public class Program
    {
        static Dictionary<string, string> testdict1 = new();
        static Dictionary<string, ConfigEntry> testdict2 = new();
        public static void Main()
        {
            //enable logging
            ConfigManager.EditInternalConfig(InternalConfigOptions.ENABLE_LOGGING, true);
            Console.WriteLine("Testing config loader API");
            ConfigData cd = new();
            testdict1.Add("#comment test", "test");
            testdict1.Add("string_test", "hi");
            testdict1.Add("bool_test", "true");
            testdict1.Add("int_test", "1");
            testdict1.Add("multi_colon_test", "SCP:SL:TEST");
            int RunFor = 1000;
            Log.WriteDebug("Running creating and loading " + RunFor.ToString() + " entries.");
            for(int i = 0; i < RunFor; i++)
            {
                testdict1.Add("key_" + i.ToString(), "value_" + i.ToString());
            }
            cd.CreateConfigFile(@".\config.txt", testdict1);
            Console.Write("Press enter to exit. . .");
            Console.Read();
        }
    }
}