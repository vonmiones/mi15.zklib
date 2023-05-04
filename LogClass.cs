using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mi15libraries
{
    public class LogClass
    {
        public static async Task WriteToFileAsync(string filePath, string text)
        {
            using (StreamWriter writer = new StreamWriter(filePath, true))
            {
                await writer.WriteLineAsync(text);
                writer.Close();
            }
        }
        public static async Task WriteToFile(string filePath, string text)
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                await writer.WriteLineAsync(text);
                writer.Close();
            }
        }

    }
}
