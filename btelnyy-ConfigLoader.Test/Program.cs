using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading.Tasks;
using btelnyy.ConfigLoader.API;
#nullable disable

namespace btelnyy.ConfigLoader.Test
{
    public class Program
    {
        public static void Main()
        {
            //enable logging
            InternalConfig.EnableLogging = true;
            Console.WriteLine("btelnyy-ConfigLoader Test Application");
            Console.WriteLine("Test App Version: " + Config.Version);
            Console.WriteLine("API Version: " + InternalConfig.Version);
            Console.WriteLine("You are in a command prompt. use 'help' for help.");
            while (true)
            {
                Console.Write("> ");
                string cmd = Console.ReadLine();
                CommandHandler.HandleCmd(cmd.Split(" "));
            }
        }
    }
}