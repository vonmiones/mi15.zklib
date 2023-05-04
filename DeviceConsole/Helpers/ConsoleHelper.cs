using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceConsole.Helpers
{
    class ConsoleHelper
    {
        public static void WriteColor(string text, ConsoleColor color)
        {
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ResetColor();
        }
        public static void WriteColor(string text, string htmlColor)
        {
            Color color = ColorTranslator.FromHtml(htmlColor);
            ConsoleColor consoleColor = GetNearestConsoleColor(color);

            Console.ForegroundColor = consoleColor;
            Console.Write(text);
            Console.ResetColor();
        }

        static ConsoleColor GetNearestConsoleColor(Color color)
        {
            ConsoleColor nearestConsoleColor = ConsoleColor.Black;
            int nearestDistance = int.MaxValue;

            foreach (ConsoleColor consoleColor in Enum.GetValues(typeof(ConsoleColor)))
            {
                Color consoleColorValue = Color.FromName(consoleColor.ToString());
                int distance = (int)Math.Pow(color.R - consoleColorValue.R, 2) +
                               (int)Math.Pow(color.G - consoleColorValue.G, 2) +
                               (int)Math.Pow(color.B - consoleColorValue.B, 2);

                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestConsoleColor = consoleColor;
                }
            }

            return nearestConsoleColor;
        }

        public static void message(string message, string status)
        {
            WriteColor(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss "), ConsoleColor.DarkBlue);
            WriteColor("[ ", ConsoleColor.DarkGreen);
            if (status.ToLower().Contains("error"))
            {
                WriteColor(status, ConsoleColor.Red);
            }
            else if (message.ToLower().Contains("fail"))
            {
                WriteColor("warning", ConsoleColor.Magenta);
            }
            else if (message.ToLower().Contains("success"))
            {
                WriteColor("success", ConsoleColor.Blue);
            }
            else
            {
                WriteColor(status, ConsoleColor.Green);
            }

            WriteColor(" ] ", ConsoleColor.DarkGreen);
            WriteColor(" :\t", ConsoleColor.Green);
            if (message.ToLower().Contains("error"))
            {
                WriteColor(message, ConsoleColor.Red);
            }
            else if (message.ToLower().Contains("fail"))
            {
                WriteColor(message, ConsoleColor.Magenta);
            }
            else if (message.ToLower().Contains("verified"))
            {
                WriteColor(message, ConsoleColor.DarkBlue);

            }
            else if (message.ToLower().Contains("success"))
            {
                WriteColor(message, ConsoleColor.Blue);
            }
            else if (message.ToLower().Contains("verify ok"))
            {
                WriteColor(message, ConsoleColor.Blue);
            }
            else
            {
                WriteColor(message, ConsoleColor.Green);
            }
            Console.WriteLine();
        }

        public static void helpinfo(string[] args)
        {
            WriteColor("Usage: ", ConsoleColor.White);
            WriteColor(" DeviceConsole.exe ", ConsoleColor.Green);
            WriteColor(args.Length > 0 ? args[0] :"", ConsoleColor.Blue);
            if (args.Length > 1)
            {
                WriteColor(args[1], ConsoleColor.Yellow);
            }else if (args.Length > 2)
            {
                WriteColor(args[2], ConsoleColor.Red);
            }
            else if (args.Length > 3)
            {
                WriteColor(args[3], ConsoleColor.Magenta);
            }  
           
            Console.WriteLine();
        }

    }
}
