using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConsole.Static
{
    class LogService
    {
        public static bool EnableLoggingService{ get;set; }
        public static void addlog(string message, string type)
        {
            if (LogService.EnableLoggingService)
            {
                type = type != "" ? type : "system";
                if (!Directory.Exists("log"))
                {
                    Directory.CreateDirectory(@"log");
                }
                using (FileStream aFile = new FileStream(@"log/" + type + ".log", FileMode.Append, FileAccess.Write))
                using (StreamWriter sw = new StreamWriter(aFile))
                {
                    sw.WriteLine(message);

                }
            }

        }
        public static void addTransactionlog(string message, string type)
        {
            type = type != "" ? type : "system";
            if (!Directory.Exists("biometric"))
            {
                Directory.CreateDirectory(@"biometric");
            }
            using (FileStream aFile = new FileStream(@"biometric/" + type + ".log", FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(aFile))
            {
                sw.WriteLine(message);

            }
        }
    }
}
