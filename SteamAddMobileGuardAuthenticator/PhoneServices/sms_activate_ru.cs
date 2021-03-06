using SteamAddMobileGuardAuthenticator.Utils;
using System;
using System.Threading;

namespace SteamAddMobileGuardAuthenticator.PhoneServices
{
    class sms_activate_ru //Site https://sms-activate.ru/
    {
        public static string APIBASEURL = "https://sms-activate.ru/stubs/handler_api.php";
        public static string steam = "mt";
        public static string OrderID = "";

        public static string getNum()
        {
            string URL = $"{APIBASEURL}?api_key={Program.config.PhoneServiceApiKey}&action=getNumber&service={steam}&country={Program.config.Country}";

            var Request = new RequestBuilder(URL).GET().Execute();

            var splitresponse = Request.Content.Split(":");

            if(splitresponse.Length == 3)
            {
                Log.info($"sms-activate.ru -- OrderID: {splitresponse[1]} Phone:{splitresponse[2]}", ConsoleColor.Yellow);

                OrderID = splitresponse[1];
                return $"+{splitresponse[2]}";
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
            if (counttryagain >= 10) return null;

            var Request = new RequestBuilder(URL).GET().Execute();

            if (Request.Content.Contains("STATUS_WAIT_CODE") || Request.Content.Contains("STATUS_WAIT_RETRY"))
            {
                Thread.Sleep(TimeSpan.FromSeconds(10));
                goto TryAgain;
            }

            if (Request.Content.Contains("STATUS_OK"))
            {
                var splitresponse = Request.Content.Split(":");
                string sms = splitresponse[1];

                if (Program.UsedSmSCodes.Contains(sms))
                {
                    Thread.Sleep(TimeSpan.FromSeconds(10));
                    goto TryAgain;
                }

                Program.UsedSmSCodes.Add(sms);
                Program.AccountsOnNumber++;
                return sms;
            }
            else return null;
        }

        public static void CancelPhone(string phone)
        {
            string URLCancel = $"{APIBASEURL}?api_key={Program.config.PhoneServiceApiKey}&action=setStatus&status=8&getStatus&id={OrderID}&forward={phone}";
            var RequestCancel = new RequestBuilder(URLCancel).GET().Execute();
            Log.info($"sms-activate.ru -- Number Canceled -- Phone:{phone}", ConsoleColor.DarkYellow);
        }

        public static void RetryNumber()
        {
            string URLCancel = $"{APIBASEURL}?api_key={Program.config.PhoneServiceApiKey}&action=setStatus&status=3&id={OrderID}";
            var RequestRetry = new RequestBuilder(URLCancel).POST().Execute();

            if (RequestRetry.Content.Contains("ACCESS_RETRY_GET"))
            {
                Log.info($"sms-activate.ru -- Retry Number -- OrderID:{OrderID}", ConsoleColor.DarkYellow);
            }
            else
            {
                Log.error($"sms-activate.ru -- Retry Number -- OrderID:{OrderID} FAILED");
            }
        }
    }
}
