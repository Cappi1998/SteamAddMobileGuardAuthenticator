using SteamAddMobileGuardAuthenticator.Utils;
using System;
using System.Threading;

namespace SteamAddMobileGuardAuthenticator.PhoneServices
{
    class cheapsms_ru //Site https://cheapsms.ru/en
    {
        public static string APIBASEURL = "https://cheapsms.pro/handler/index";
        public static string steam = "mt";
        public static string OrderID = "";

        public static string getNum()
        {
            string URL = $"{APIBASEURL}?api_key={Program.config.PhoneServiceApiKey}&action=getNumber&service={steam}&country={Program.config.Country}";
            var Request = new RequestBuilder(URL).GET().Execute();

            var splitresponse = Request.Content.Split(":");

            if (splitresponse.Length == 3)
            {
                Log.info($"cheapsms.ru -- OrderID: {splitresponse[1]} Phone:{splitresponse[2]}", ConsoleColor.Yellow);

                OrderID = splitresponse[1];
                return $"{splitresponse[2]}";
            }
            else
            {
                return null;
            }
        }

        public static string Getcode()
        {
            string URL = $"{APIBASEURL}?api_key={Program.config.PhoneServiceApiKey}&action=getStatus&id={OrderID}";

            int counttryagain = 0;

        TryAgain:

            counttryagain++;

            if (counttryagain >= 10)
            {
                return null;
            }


            var Request = new RequestBuilder(URL).GET().Execute();

            if (Request.Content.Contains("STATUS_WAIT_CODE"))
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                goto TryAgain;
            }

            if (Request.Content.Contains("STATUS_OK"))
            {
                var splitresponse = Request.Content.Split(":");

                return splitresponse[1];
            }
            else
            {
                return null;
            }
        }

        public static void CancelPhone(string phone)
        {
            string URLCancel = $"{APIBASEURL}?api_key={Program.config.PhoneServiceApiKey}&action=setStatus&status=8&getStatus&id={OrderID}&forward={phone}";
            var RequestCancel = new RequestBuilder(URLCancel).GET().Execute();
            Log.info($"cheapsms.ru -- Number Canceled -- Phone:{phone}", ConsoleColor.DarkYellow);
        }
    }
}
