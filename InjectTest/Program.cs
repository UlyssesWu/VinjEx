using System;
using System.Windows.Forms;
using VinjEx;
using System.Diagnostics;

namespace InjectTest
{
    class Program
    {
        private static void Main(string[] args)
        {
            int pid = 0;
            Console.WriteLine("Input process name:");
            string processName = Console.ReadLine();

            var ps = Process.GetProcessesByName(processName);
            bool getted = false;
            foreach (var process in ps)
            {
                if (string.IsNullOrEmpty(process.MainWindowTitle)) continue;
                pid = process.Id;
                getted = true;
                break;
            }
            if (!getted)
            {
                Console.WriteLine("Can not find that process!");
                Console.ReadLine();
                return;
            }

            InjectableProcess ip = new InjectableProcess(pid);
            ip.OnClientResponse += command => MessageBox.Show("[Host]Got a message from client:\n" + command.ToString(),Process.GetCurrentProcess().ProcessName);
            if (ip.Inject(@"TestDLL.dll") == 0)
            {
                Console.WriteLine("Failed to inject!");
                Console.ReadLine();
                return;
            }

            ip.Command("This was a triumph.");
            Console.ReadLine();
            ip.Command("VinjEx by Ulysses - wdwxy12345@gmail.com");
            Console.ReadLine();
            ip.Eject();
        }
    }

}
