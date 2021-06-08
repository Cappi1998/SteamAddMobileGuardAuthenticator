using Add_MobileGuard.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Add_MobileGuard
{
    class FolderStartupCheck
    {

        public static void Check()
        {
            if (!Directory.Exists(Program.Database_Path))
            {
                Directory.CreateDirectory(Program.Database_Path);
            }
            if (!Directory.Exists(Path.Combine(Program.Database_Path, "Accounts_ToAdded_Guard")))
            {
                Directory.CreateDirectory(Path.Combine(Program.Database_Path, "Accounts_ToAdded_Guard"));
            }
            if (!Directory.Exists(Path.Combine(Program.Database_Path, "Mobile_Guard_Added")))
            {
                Directory.CreateDirectory(Path.Combine(Program.Database_Path, "Mobile_Guard_Added"));
            }

            string ConfigFile = Path.Combine(Program.Database_Path, "Config.json");

            if (!File.Exists(ConfigFile))
            {
                Config config = new Config { ApiKey = "YouApiKey", Country=7, PhoneServiceToUse = "onlinesim.ru" };

                File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(config, Formatting.Indented));
            }
        }
    }
}
