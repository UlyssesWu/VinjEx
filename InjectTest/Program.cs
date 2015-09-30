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
            //Get a process by name
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
            //Get ready to inject
            InjectableProcess ip = new InjectableProcess(pid);
            //Register a method to handle DLL's response
            //Always register methods BEFORE DLL injection
            ip.OnClientResponse += command => MessageBox.Show("[Host]Got a message from client:\n" + command.ToString(),Process.GetCurrentProcess().ProcessName);
            ip.OnClientExit += () => { MessageBox.Show("[Host]Got client offline message.\nNow I only Want You Gone-"); };
            //Inject method would return 0 If inject failed (same as VInjDn do)
            if (ip.Inject(@"TestDLL.dll") == 0)
            {
                Console.WriteLine("Failed to inject!");
                Console.ReadLine();
                return;
            }
            //Send command to DLL
            ip.Command("This was a triumph.");
            Console.ReadLine();
            //Commands to test by
            ip.Command("VinjEx by Ulysses - wdwxy12345@gmail.com");
            Console.ReadLine();
            //Use this to release DLL 
            ip.Eject();
            Console.ReadLine();
        }
    }

}
