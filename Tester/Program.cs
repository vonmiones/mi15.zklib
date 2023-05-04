using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            string filePath = @args[0];

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = filePath;
            startInfo.Arguments = args[1]; // optional arguments to pass to the other app
            startInfo.WindowStyle = ProcessWindowStyle.Normal; // optional window style

            Process process = new Process();
            process.StartInfo = startInfo;

            process.Start();
            process.WaitForExit();
        }
    }
}
