using Add_MobileGuard.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Add_MobileGuard.PhoneServices
{
    class sms_activate_ru //Site https://sms-activate.ru/
    {
        public static string APIBASEURL = "https://sms-activate.ru/stubs/handler_api.php";
        public static string steam = "mt";
        public static string OrderID = "";

        public static string getNum()
        {
            string URL = $"{APIBASEURL}?api_key={Program.config.ApiKey}&action=getNumber&service={steam}&country={Program.config.Country}";

            var Request = new RequestBuilder(URL).GET().Execute();

            var splitresponse = Request.Content.Split(":");

            if(splitresponse.Length == 3)
            {
                Log.info($"sms-activate.ru: OrderID: {splitresponse[1]} Phone:{splitresponse[2]}", ConsoleColor.Yellow);

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
            string URL = $"{APIBASEURL}?api_key={Program.config.ApiKey}&action=getStatus&id={OrderID}";

            int counttryagain = 0;

        TryAgain:

            counttryagain++;

            if(counttryagain >= 10)
            {
                return null;
            }


            var Request = new RequestBuilder(URL).GET().Execute();

            if (Request.Content.Contains("STATUS_WAIT_CODE"))
            {
                Thread.Sleep(TimeSpan.FromSeconds(30));
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
    }
}
