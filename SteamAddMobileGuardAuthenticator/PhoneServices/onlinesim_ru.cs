using Newtonsoft.Json.Linq;
using SteamAddMobileGuardAuthenticator.Utils;
using System;
using System.Threading;

namespace SteamAddMobileGuardAuthenticator.PhoneServices
{
    class onlinesim
    {
        public static string APIBASEURL = "https://onlinesim.ru/api/";
        public static string steam = "steam";

        public static string getNum()
        {
            Inicio:
            string URL = $"{APIBASEURL}getNum.php?apikey={Program.config.PhoneServiceApiKey}&service={steam}&number=true&country=372";

            var Request = new RequestBuilder(URL)
                .GET()
                .Execute();

            if (Request.Content.Contains("WARNING_LOW_BALANCE"))
            {
                Log.error("WARNING_LOW_BALANCE");
                Log.info("Add more balance and press enter to continue!", ConsoleColor.DarkYellow);
                Console.ReadLine();
                goto Inicio;
            }

            if (Request.Content.Contains("ERROR_NO_OPERATIONS") || Request.Content.Contains("TRY_AGAIN_LATER") || Request.Content.Contains("NO_NUMBER"))
            {
                Log.error("Erro To get new phone number:");
                Log.error(Request.Content);
                Thread.Sleep(8000);
                goto Inicio;
            }

            dynamic json = JValue.Parse("{\"Valid_Json\":\"Valid_Json\"}");//para não ficar em branco

            try
            {
                json = JValue.Parse(Request.Content);
            }
            catch(Exception ex)
            {
                Log.error(ex.Message, ex);
                return "";
            }

            string Number = Convert.ToString(json.number.Value);

            if (!string.IsNullOrEmpty(Number))
            {
                Program.numero = Number;
                Program.tzid = Convert.ToInt32(json.tzid);
                return Number;
            }
               

            return "";
        }

        public static string Getcode()
        {
            int tentativas = 0;
            INICIO:

            if(tentativas > 9)
            {
                Log.error($"SMS not received on phone:{Program.numero}");
                return "";
            }

            string URL = $"{APIBASEURL}getState.php?apikey={Program.config.PhoneServiceApiKey}&tzid={Program.tzid}&message_to_code=1&msg_list=1";

            var Request = new RequestBuilder(URL)
                .GET()
                .Execute();

            dynamic json = JValue.Parse("{\"Valid_Json\":\"Valid_Json\"}");//para não ficar em branco

            if(Request.Content.Contains("TRY_AGAIN_LATER"))
            {
                Log.error($"Error to get code from phone: {Program.numero}");
                Log.error($"Response:{Request}");
                Thread.Sleep(15000);
                tentativas++;
                goto INICIO;
            }


            if (Request.Content.Contains("ERROR_NO_OPERATIONS"))
            {
                Log.error($"Error to get code from phone: {Program.numero}");
                Log.error($"Response:{Request}");
                Thread.Sleep(4000);
                tentativas++;
                goto INICIO;
            }

            try
            {
                json = JValue.Parse(Request.Content);
            }
            catch (Exception ex)
            {
                Log.error(ex.Message, ex);
                return "";
            }

            

            if(json[0].response.Value == "TZ_NUM_WAIT")
            {
                Log.info("TZ_NUM_WAIT", ConsoleColor.Yellow);
                Thread.Sleep(60000);
                tentativas++;
                goto INICIO;
            }

            int ultimorecebido = json[0].msg.Count;

            if (ultimorecebido == 1)
            {
                Thread.Sleep(15000);
                goto INICIO;
            }
            else
            {
                string codigo = json[0].msg[ultimorecebido - 1].msg.Value;

                if (!string.IsNullOrEmpty(codigo))
                {
                    return codigo;
                }
                goto INICIO;
            }
        }

        public static void getNumRepeat(string Numero)
        {

        }
    }
}
