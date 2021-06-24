using Add_MobileGuard.Models;
using Add_MobileGuard.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;

namespace Add_MobileGuard
{
    class Program
    {
        public static string Database_Path = AppDomain.CurrentDomain.BaseDirectory + "Database\\";
        public static string Pop3Domains_Path = Path.Combine(Program.Database_Path, "Pop3Domains.json");
        public static string Accounts_ToAdded_Guard = Database_Path + "Accounts_ToAdded_Guard\\";
        public static string Mobile_Guard_Added = Database_Path + "Mobile_Guard_Added\\";

        public static Config config = null;
        public static string numero = "";

        public static int tzid = 0;
        public static List<Pop3> pop3s = new List<Pop3>();
        static void Main(string[] args)
        {
            Console.Title = "Add_MobileGuard // Cappi_1998";
            FolderStartupCheck.Check();
            pop3s = JsonConvert.DeserializeObject<List<Pop3>>(File.ReadAllText(Pop3Domains_Path));
            config = JsonConvert.DeserializeObject<Config>(File.ReadAllText(Path.Combine(Database_Path,"Config.json")));

            if(config.PhoneServiceApiKey == "YouApiKey")
            {
                Log.error("Configure the /Database/Config.json file and open the program again!");
                Console.ReadLine();
            }
            else
            {
                MobileGuard_AddedProcess.StartProcess();
            }
        }
    }
}
