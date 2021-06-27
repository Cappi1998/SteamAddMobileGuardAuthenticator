using Add_MobileGuard.Models;
using Newtonsoft.Json;
using SteamAddMobileGuardAuthenticator.PhoneServices;
using SteamAddMobileGuardAuthenticator.Utils;
using SteamAuth;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;

namespace SteamAddMobileGuardAuthenticator
{
    class MobileGuard_AddedProcess
    {
        public static void StartProcess()
        {

            if(Program.config.AccountsFormatInput == "SteamAccountCreateHelper")
            {
                string[] Accoun_files_path = Directory.GetFiles(Program.Accounts_ToAdded_Guard, "*.txt");

                int counter = 0;
                foreach (string currentFile in Accoun_files_path)
                {
                    counter++;
                    Log.info($"Processing {counter}/{Accoun_files_path.Length}", ConsoleColor.Cyan);

                    var Account_File_text = File.ReadAllText(currentFile);

                    string[] all_lines = Regex.Split(Account_File_text, "[\r\n]+");
                    string first_line = new StringReader(Account_File_text).ReadLine();

                    var split = first_line.Split(':');
                    var get_mail_split = all_lines[1].Split(' ');
                    var get_mailPASS_split = all_lines[2].Split(' ');
                    string Username = split[0];
                    string Pass = split[1];
                    string E_Mail = get_mail_split[1];
                    string E_Mail_Pass = get_mailPASS_split[2];


                    Add_GuardMobile(currentFile, Username, Pass, E_Mail, E_Mail_Pass);
                }

            }else if(Program.config.AccountsFormatInput == "login:pass:email:emailpass")
            {
                string[] AccountsAndEmailList = File.ReadAllLines(Path.Combine(Program.Database_Path, "Accounts_ToAdded_Guard.txt"));

                int counter = 0;
                foreach (string AccountsInfo in AccountsAndEmailList)
                {
                    try
                    {
                        counter++;
                        Log.info($"Processing {counter}/{AccountsAndEmailList.Length}", ConsoleColor.Cyan);

                        var split = AccountsInfo.Split(':');
                        string Username = split[0];
                        string Pass = split[1];
                        string E_Mail = split[2];
                        string E_Mail_Pass = split[3];

                        Add_GuardMobile(null, Username, Pass, E_Mail, E_Mail_Pass);
                    }
                    catch (Exception ex)
                    {
                        Log.error(ex.Message);
                        Log.error($"Format input is Invalid, correct format is \"login:pass:email:emailpass\"");
                    }
                }
            }
            else
            {
                Log.error($"{Program.config.AccountsFormatInput} is not valid for config.AccountsFormatInput.");
                Log.info($"Use \"SteamAccountCreateHelper\" or \"login:pass:email:emailpass\"", ConsoleColor.DarkYellow);
                Log.error("If you have any more questions read our #Readme.");
                Log.info("https://github.com/Cappi1998/SteamAddMobileGuardAuthenticator#readme", ConsoleColor.Blue);
            }

            Log.info("All is Done!! ", ConsoleColor.Cyan);
            Console.ReadKey();
        }

        public static void Add_GuardMobile(string File_Path, string username, string pass, string E_mail, string E_mail_pass)
        {

            Log.info("Connected to Steam! Logging in " + username, ConsoleColor.Blue);

            UserLogin autoaccept = new UserLogin(username, pass);


            List<string> EmailCodes = new List<string>();
            int getEmailCount = 0;

            LoginResult response = LoginResult.BadCredentials;
            while ((response = autoaccept.DoLogin()) != LoginResult.LoginOkay)
            {
                switch (response)
                {
                    case LoginResult.NeedEmail:
                        {
                        Again:
                            getEmailCount++;
                            E_Mail e_Mail = new E_Mail { EMAIL = E_mail, EMAIL_PASS = E_mail_pass };
                            string Mail_Code = Get_Email_Code_Pop3.Get_code_mail(e_Mail);
                            Log.info("E-Mail Guard Code: " + Mail_Code, ConsoleColor.DarkGreen);


                            if (getEmailCount >= 7)
                            {
                                Log.error("E-Mail Guard Code Not Found!");
                                return;
                            }
                                

                            if (EmailCodes.Contains(Mail_Code))
                            {
                                goto Again;
                            }

                            EmailCodes.Add(Mail_Code);
                            getEmailCount++;
                            autoaccept.EmailCode = Mail_Code;

                            break;
                        }

                    case LoginResult.BadCredentials:
                        {
                            Log.error("ERROR - BadCredentials!");
                            break;
                        }

                    case LoginResult.GeneralFailure:
                        {

                            Log.error("ERROR - GeneralFailure!");
                            return;
                        }

                    case LoginResult.TooManyFailedLogins:
                        {
                            Log.error("ERROR - TooManyFailedLogins Please Waint!");
                            break;
                        }

                    case LoginResult.NeedCaptcha:
                        {
                            Process myProcess = new Process();
                            myProcess.StartInfo.UseShellExecute = true;
                            myProcess.StartInfo.FileName = "https://help.steampowered.com/pt-br/login/rendercaptcha/?gid=" + autoaccept.CaptchaGID;
                            myProcess.Start();

                            Log.info("Login Need Captcha, try Get Captcha Code!!", ConsoleColor.DarkYellow);
                            Log.info("Please Inform the Captcha, and press enter:", ConsoleColor.DarkYellow);
                            string Code = Console.ReadLine();
                            autoaccept.CaptchaText = Code;
                            break;
                        }

                    case LoginResult.Need2FA:
                        {
                            Log.error("ERROR - Need2FA!");
                            return;
                        }
                }
            }

            Log.info("Login Sucess in Account: " + username, ConsoleColor.DarkGreen);

            string num = null;

            SessionData session = autoaccept.Session;
            AuthenticatorLinker linker = new AuthenticatorLinker(session);
            AuthenticatorLinker.LinkResult linkResponse = AuthenticatorLinker.LinkResult.GeneralFailure;

            while ((linkResponse = linker.AddAuthenticator()) != AuthenticatorLinker.LinkResult.AwaitingFinalization)
            {
                switch (linkResponse)
                {
                    case AuthenticatorLinker.LinkResult.MustProvidePhoneNumber:
                        {
                            GetNum:
                            num = GetPhoneNum();
                            if (string.IsNullOrWhiteSpace(num))
                            {
                                goto GetNum;
                            }

                            Program.numero = num;
                            Log.info("Get Phone Number: " + num, ConsoleColor.Yellow);
                            linker.PhoneNumber = num;
                            break;
                        }

                    case AuthenticatorLinker.LinkResult.MustRemovePhoneNumber:
                        {
                            linker.PhoneNumber = null;
                            Log.error("Error adding your phone number. Steam returned \"MustRemovePhoneNumber\".");
                            return;
                        }


                    case AuthenticatorLinker.LinkResult.MustConfirmEmail:
                        {
                            Thread.Sleep(TimeSpan.FromSeconds(10));
                            E_Mail e_Mail = new E_Mail { EMAIL = E_mail, EMAIL_PASS = E_mail_pass };

                            var link = Get_Email_Code_Pop3.Get_URL_Confirm(e_Mail);

                            if(link == "")
                            {
                                Log.error("Email Confirmation Link Not Found!");
                                CancelPhone(num);
                                return;
                            }

                            var request = new RequestBuilder(link).GET().Execute();
                            Thread.Sleep(TimeSpan.FromSeconds(5));

                            break;
                        }


                    case AuthenticatorLinker.LinkResult.AwaitingFinalization:
                        {
                            break;
                        }

                    case AuthenticatorLinker.LinkResult.GeneralFailure:
                        {
                            Log.error("Error adding your phone number. Steam returned \"GeneralFailure\".");
                            CancelPhone(num);
                            return;
                        }
                }
            }

            if(Program.numero == "")
            {
                Log.error($"Error Phone number linked to the account, please remove it and try again");
                return;
            }

            string code = null;

            switch (Program.config.PhoneServiceToUse)
            {
                case "sms-activate.ru"://sms-activate.ru
                    {
                        code = sms_activate_ru.Getcode();
                        break;
                    }
                case "cheapsms.ru"://cheapsms.ru
                    {
                        code = cheapsms_ru.Getcode();
                        break;
                    }
                    
                default:
                    {
                        Log.error($"Config.PhoneServiceToUse: {Program.config.PhoneServiceToUse} is Invalid!");
                        break;
                    }
            }


            if(string.IsNullOrEmpty(code))
            {
                Log.info("Phone Code is null!", ConsoleColor.Red);
                CancelPhone(num);
                return;
            }

            Log.info($"SmS Code: {code}", ConsoleColor.DarkMagenta);
            string fileName = Path.Combine(Program.Mobile_Guard_Added, $"{username}.maFile");

            try
            {
                string sgFile = JsonConvert.SerializeObject(linker.LinkedAccount, Formatting.Indented);

                File.WriteAllText(fileName, sgFile);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Console.WriteLine("EXCEPTION saving maFile. For security, authenticator will not be finalized.");

            }

            SteamAuth.AuthenticatorLinker.FinalizeResult linkeresponse = linker.FinalizeAddAuthenticator(code);


            if (linkeresponse == SteamAuth.AuthenticatorLinker.FinalizeResult.Success)
            {
                Log.info($"Added Guard Success: {username}", ConsoleColor.Green);

                if(Program.config.AccountsFormatInput == "SteamAccountCreateHelper")
                {
                    var textoarquivo = File.ReadAllText(File_Path);

                    StreamWriter sw;
                    sw = File.AppendText(Path.Combine(Program.Mobile_Guard_Added, $"{username}.txt"));

                    sw.WriteLine(textoarquivo +
                        $"\r\nPhone: {linker.PhoneNumber}" +
                        $"\r\nR_CODE: {linker.LinkedAccount.RevocationCode}"
                        );

                    sw.Close();
                    sw.Dispose();

                    if (File_Path != null) File.Delete(File_Path);
                }
                else if (Program.config.AccountsFormatInput == "login:pass:email:emailpass")
                {
                    StreamWriter sw;
                    sw = File.AppendText(Path.Combine(Program.Mobile_Guard_Added, $"{username}.txt"));

                    sw.WriteLine(
                        $"{username}:{pass}" +
                        $"\r\n\r\n" +
                        $"Email: {E_mail}" +
                        $"\r\n" +
                        $"EMail Password: {E_mail_pass}" +
                        $"\r\n\r\n" +
                        $"https://steamcommunity.com/profiles/{autoaccept.Session.SteamID}" +
                        $"\r\n\r\n" +
                        $"Phone: {linker.PhoneNumber}" +
                        $"\r\n" +
                        $"R_CODE: {linker.LinkedAccount.RevocationCode}"
                        );

                    sw.Close();
                    sw.Dispose();

                    RemoveAccountLineFromTxT(username);
                }
            }
            else
            {
                Log.error($"Error to Added Guard: {linkeresponse}");
                try
                {
                    File.Delete(fileName);
                }
                catch
                {

                }
            }

        }

        public static void CancelPhone(string num)
        {
            Program.numero = null;
            switch (Program.config.PhoneServiceToUse)
            {
                case "onlinesim.ru":
                    {
                        //onlinesim.CancelPhone();
                        break;
                    }
                case "sms-activate.ru":
                    {
                        sms_activate_ru.CancelPhone(num);
                        break;
                    }
                case "cheapsms.ru":
                    {
                        cheapsms_ru.CancelPhone(num);
                        break;
                    }

                default:
                    {
                        Log.error($"Config.PhoneServiceToUse: {Program.config.PhoneServiceToUse} is Invalid!");
                        break;
                    }
            }
        }

        public static string GetPhoneNum()
        {
            if(Program.AccountsOnNumber >= Program.config.AccountsPerNumber || string.IsNullOrWhiteSpace(Program.numero))
            {
                Program.AccountsOnNumber = 0;
                switch (Program.config.PhoneServiceToUse)
                {
                    case "sms-activate.ru":
                        {
                            return sms_activate_ru.getNum();
                        }

                    case "cheapsms.ru":
                        {
                            return cheapsms_ru.getNum();
                        }

                    default:
                        {
                            Log.error("Config.PhoneServiceToUse is Invalid!");
                            break;
                        }
                }
            }

            switch (Program.config.PhoneServiceToUse)
            {
                case "sms-activate.ru":
                    {
                        sms_activate_ru.RetryNumber();
                        return Program.numero;
                    }

                case "cheapsms.ru":
                    {
                        cheapsms_ru.RetryNumber();
                        return Program.numero;
                    }

                default:
                    {
                        Log.error("Config.PhoneServiceToUse is Invalid!");
                        break;
                    }
            }
            return Program.numero;
        }

        public static void RemoveAccountLineFromTxT(string AccountName)
        {
            string path = Path.Combine(Program.Database_Path, "Accounts_ToAdded_Guard.txt");

            List<string> AccountsAndEmailList = File.ReadAllLines(path).ToList();

            int count = 0;
            foreach(var line in AccountsAndEmailList)
            {
                
                if (line.Contains(AccountName))
                {
                    AccountsAndEmailList.RemoveAt(count);
                    File.WriteAllLines(path, AccountsAndEmailList);
                    return;
                }
                count++;
            }
        }
    }
}
