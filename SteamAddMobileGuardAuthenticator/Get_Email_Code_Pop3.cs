using System;
using System.Linq;
using MailKit.Net.Pop3;
using System.Text.RegularExpressions;
using System.Threading;
using Add_MobileGuard.Models;

namespace SteamAddMobileGuardAuthenticator
{
    class Get_Email_Code_Pop3
    {
        public static Pop3Client Pop3Client;

        public static string Get_code_mail(E_Mail mail)
        {
            int tentativas_get_code = 0; INICIO:
            string Guard_Code = "";

            string Pop3HostName = "";
            string domainName = mail.EMAIL.Split('@')[1];

            var popconfig = Program.pop3s.Where(a => a.SuportedDomains.Contains(domainName)).FirstOrDefault();

            if (popconfig != null)
            {
                Pop3HostName = popconfig.PoP3Server;
            }
            else
            {
                Log.error($"Pop3 HostName not defined for {domainName}");
                return null;
            }

            string username = mail.EMAIL;
            string password = mail.EMAIL_PASS;


            using (var client = new Pop3Client())
            {
                try
                {
                    client.Connect(Pop3HostName, 995, true);

                    client.Authenticate(username, password);

                    if (client.IsAuthenticated == true)
                    {
                        Pop3Client = client;
                    }
                }
                catch (Exception ex)
                {
                    Log.error("Error to acess E-mail: " + username);
                    Log.error(ex.Message);

                }

                while (Guard_Code == "")
                {

                    if (tentativas_get_code < 3)
                    {
                        tentativas_get_code++;
                        Thread.Sleep(TimeSpan.FromSeconds(15));
                        try
                        {

                            var message1 = client.GetMessage(client.Count - 1);

                            Guard_Code = new Regex("\\w{5}(?=											<\\/td>)").Match(message1.HtmlBody.ToString()).Value;

                        }
                        catch
                        {
                            Log.info("Erro To get E-mail Confirmation Link. Try Again!!", ConsoleColor.DarkMagenta);
                            Thread.Sleep(TimeSpan.FromSeconds(15));
                            tentativas_get_code = tentativas_get_code + 1;
                            client.Disconnect(true);
                            goto INICIO;
                        }
                    }
                    else
                    {
                        Log.error("Erro To get Guard Code from E-mail!");
                        return Guard_Code;
                    }
                }

                client.Disconnect(true);

            }

            return Guard_Code;
        }

        public static string Get_URL_Confirm(E_Mail mail)
        {
            int tentativas_get_code = 0; INICIO:
            string Confirm_Link = "";

            string Pop3HostName = "";
            string domainName = mail.EMAIL.Split('@')[1];

            var popconfig = Program.pop3s.Where(a => a.SuportedDomains.Contains(domainName)).FirstOrDefault();

            if (popconfig != null)
            {
                Pop3HostName = popconfig.PoP3Server;
            }
            else
            {
                Log.error($"Pop3 HostName not defined for {domainName}");
                return null;
            }

            string username = mail.EMAIL;
            string password = mail.EMAIL_PASS;


            using (var client = new Pop3Client())
            {


                try
                {

                    client.Connect(Pop3HostName, 995, true);

                    client.Authenticate(username, password);

                    if (client.IsAuthenticated == true)
                    {
                        Pop3Client = client;
                    }
                }
                catch (Exception ex)
                {
                    Log.error("Error to acess E-mail: " + username);
                    Log.error(ex.Message);

                }

                while (Confirm_Link == "")
                {

                    if (tentativas_get_code < 6)
                    {
                        try
                        {

                            var message1 = client.GetMessage(client.Count - 1);



                            var Dados = new Regex("(?<=stoken\\=)\\S+").Match(message1.Body.ToString()).Value;

                            var split = Dados.Split('&');
                            var spli2 = split[1].Split('=');

                            string stoken = split[0];
                            string steamid = spli2[1];

                            Confirm_Link = "https://store.steampowered.com/phone/ConfirmEmailForAdd?stoken=" + stoken + "&steamid=" + steamid;

                        }
                        catch
                        {
                            Log.info("Erro To get E-mail Confirmation Link. Try Again!!", ConsoleColor.DarkMagenta);
                            Thread.Sleep(TimeSpan.FromSeconds(15));
                            tentativas_get_code = tentativas_get_code + 1;
                            client.Disconnect(true);
                            goto INICIO;
                        }
                    }
                    else
                    {
                        Log.error("Erro To get E-mail Confirmation Link!");
                        return Confirm_Link;
                    }
                }

                client.Disconnect(true);
            }

            return Confirm_Link;
        }
    }
}
