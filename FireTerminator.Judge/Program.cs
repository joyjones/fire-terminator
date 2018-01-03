using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace FireTerminator.Judge
{
    class Program
    {
        static void Main(string[] args)
        {
            var files = Directory.GetFiles(System.Environment.CurrentDirectory, "FireTerminator.Client.exe", SearchOption.TopDirectoryOnly);
            if (files.Length > 0)
            {
                var pi = new ProcessStartInfo(files[0], "JudgeMode=1");
                pi.WorkingDirectory = System.Environment.CurrentDirectory;
                Process.Start(pi);
            }
        }
    }
}
