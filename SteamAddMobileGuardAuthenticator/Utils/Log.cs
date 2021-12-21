using System;
using System.IO;

namespace SteamAddMobileGuardAuthenticator
{
    public class Log
    {
        private static readonly object locker = new object();

        public static void info(string msg, ConsoleColor color)
        {
            lock (locker)
            {
                Console.ForegroundColor = color;
                msg = DateTime.Now + " - " + msg;
                Console.WriteLine(msg);
                Console.ResetColor();

                StreamWriter sw;
                sw = File.AppendText(Program.Database_Path + "log.txt");
                sw.WriteLine(msg);
                sw.Close();
                sw.Dispose();
            }
        }

        public static void error(string msg, Exception e)
        {
            lock (locker)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                msg = DateTime.Now + " - " + msg + ". " + e.Message;
                Console.WriteLine(msg);
                Console.ResetColor();

                StreamWriter sw;
                sw = File.AppendText(Program.Database_Path + "error.txt");
                sw.WriteLine(msg);
                sw.Close();
                sw.Dispose();

                StreamWriter sw1;
                sw1 = File.AppendText(Program.Database_Path + "error.txt");
                sw1.WriteLine(msg + "\n" + e.StackTrace);
                sw1.Close();
                sw1.Dispose();
            }
        }

        public static void error(string msg)
        {
            lock (locker)
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                msg = DateTime.Now + " - " + msg;
                Console.WriteLine(msg);
                Console.ResetColor();

                StreamWriter sw;
                sw = File.AppendText(Program.Database_Path + "error.txt");
                sw.WriteLine(msg);
                sw.Close();
                sw.Dispose();
            }
        }
    }
}