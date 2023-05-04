using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DeviceConsole.Static
{
    public static class FileHandlingClass
    {
        public static void WriteLogFile(string path,string message,string type)
        {
            type = type != "" ? type : "system";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(@path);
            }
            using (FileStream aFile = new FileStream(@path+ type + ".log", FileMode.Append, FileAccess.Write))
            using (StreamWriter sw = new StreamWriter(aFile))
            {
                sw.WriteLine(message);
            }
        }

        public static List<string> rsearch(string searchPattern)
        {
            List<string> fileList = new List<string>();
            string currentDir = Directory.GetCurrentDirectory();

            // Replace each '#' character with a regular expression pattern that matches a digit
            string regexPattern = Regex.Escape(searchPattern).Replace(@"\#", @"\d");

            try
            {
                // Search for files that match the regular expression pattern
                foreach (string file in Directory.EnumerateFiles(currentDir, "*", SearchOption.AllDirectories))
                {
                    if (Regex.IsMatch(Path.GetFileName(file), regexPattern))
                    {
                        fileList.Add(file);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occurred while searching for files: " + e.Message);
            }

            return fileList;
        }
    }
}
