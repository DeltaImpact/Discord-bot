using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Linq;
using Serilog;

namespace DSPlus.Examples
{
    static class Logger
    {
        //static string path = "files/log.txt";

        //private static FileStream fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
        //static StreamWriter writer;

        //static Logger()
        //{
        //    writer = new StreamWriter(path, true);
        //    writer.Close();
        //}

        public static void SaveString(string message)
        {
            Log.Information(message);
            //writer.WriteLine(message);
        }

        public static void SaveEvent(string who, string what)
        {
            string when = DateTime.Now.ToString();
            //string when = DateTime.Now.ToString("dd MMMM yyyy | HH:mm:ss");

            string message = who + " | " + what + " | " + when;
            //Console.WriteLine(message);

            Log.Information(message);

            //writer.WriteLine(message);
            //writer.Flush();
        }



    }
}
