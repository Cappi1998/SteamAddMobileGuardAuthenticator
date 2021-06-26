using Add_MobileGuard.Models;
using Newtonsoft.Json;
using SteamAddMobileGuardAuthenticator.Utils;
using System.Collections.Generic;
using System.IO;

namespace SteamAddMobileGuardAuthenticator
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
                Config config = new Config { AccountsFormatInput= "SteamAccountCreateHelper", PhoneServiceApiKey = "YouApiKey", Country=0, PhoneServiceToUse = "onlinesim.ru", AccountsPerNumber=2};

                File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(config, Formatting.Indented));
            }

            string Pop3File = Path.Combine(Program.Database_Path, "Pop3Domains.json");

            if (!File.Exists(Pop3File))
            {
                List<Pop3> pop3s = new List<Pop3> { 
                    new Pop3 {PoP3Server="pop.gmail.com", SuportedDomains= new List<string> { "gmail.com" } },
                    new Pop3 {PoP3Server="pop.yandex.ru", SuportedDomains= new List<string>{ "yandex.ru" } },
                    new Pop3 {PoP3Server="pop.mail.ru", SuportedDomains=new List<string>{ "mail.ru" } },
                    new Pop3 {PoP3Server="pop.rambler.ru", SuportedDomains= new List<string>{ "rambler.ru", "ro.ru", "myrambler.ru", "lenta.ru", "autorambler.ru" } }
                    
                };
                File.WriteAllText(Pop3File, JsonConvert.SerializeObject(pop3s, Formatting.Indented));
            }
        }
    }
}
