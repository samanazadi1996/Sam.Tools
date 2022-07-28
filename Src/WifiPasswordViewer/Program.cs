using System;
using System.Diagnostics;
using System.Linq;

namespace WifiPasswordViewer
{
    class Program
    {
        static void Main(string[] args)
        {
            Print(ConsoleColor.White, "{0}  {1, 30}", "Wifi Name", "Password");
            Print(ConsoleColor.White, "-----------------------------------------");

            var result = RunCmd("netsh wlan show profile")
                .Split(Environment.NewLine)
                .Where(p => p.Contains("All User Profile"))
                .Select(p => p.Substring(p.LastIndexOf(":") + 2));

            foreach (var item in result)
            {
                var password = RunCmd($"netsh wlan show profile name= “{item}” key=clear")
                .Split(Environment.NewLine)
                .Where(p => p.Contains("Key Content"))
                .Select(p => p.Substring(p.LastIndexOf(":") + 2)).FirstOrDefault();
                Print((ConsoleColor)new Random().Next(1, 14), "{0}  {1, 30}", item, password);
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Print(ConsoleColor.White, "Press Any Key To Exit ...");
            Console.ReadKey();
        }

        static void Print(ConsoleColor color, string format, params object[] arg)
        {
            Console.ForegroundColor = color; 
            Console.WriteLine(format, arg);
        }
        static void Print(ConsoleColor color, string text)
        {
            Console.ForegroundColor = color;
            Console.WriteLine(text);
        }
        static string RunCmd(string strCmdText)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.RedirectStandardInput = true;
            cmd.StartInfo.RedirectStandardOutput = true;
            cmd.StartInfo.CreateNoWindow = true;
            cmd.StartInfo.UseShellExecute = false;
            cmd.Start();
            cmd.StandardInput.WriteLine(strCmdText);
            cmd.StandardInput.Flush();
            cmd.StandardInput.Close();
            cmd.WaitForExit();
            return cmd.StandardOutput.ReadToEnd();
        }
    }
}
