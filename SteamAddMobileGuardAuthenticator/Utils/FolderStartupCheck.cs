﻿using Add_MobileGuard.Models;
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
                Config config = new Config { PhoneServiceApiKey = "YouApiKey", Country=7, PhoneServiceToUse = "onlinesim.ru" };

                File.WriteAllText(ConfigFile, JsonConvert.SerializeObject(config, Formatting.Indented));
            }

            string Pop3File = Path.Combine(Program.Database_Path, "Pop3Domains.json");

            if (!File.Exists(Pop3File))
            {
                List<Pop3> pop3s = new List<Pop3> { 
                    new Pop3 {PoP3Server= "pop.gmail.com", SuportedDomains= new List<string> { "gmail.com" } },
                    new Pop3 {PoP3Server="pop.yandex.ru", SuportedDomains= new List<string>{ "yandex.ru" } },
                    new Pop3 {PoP3Server="pop.mail.ru", SuportedDomains=new List<string>{ "mail.ru" } },
                    new Pop3 {PoP3Server="pop.rambler.ru", SuportedDomains= new List<string>{ "rambler.ru", "ro.ru", "myrambler.ru" } }
                    
                };
                File.WriteAllText(Pop3File, JsonConvert.SerializeObject(pop3s, Formatting.Indented));
            }
        }
    }
}
