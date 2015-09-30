using System;
using System.Windows.Forms;
using VinjEx;
using System.Diagnostics;

namespace InjectTest
{
    /// <summary>
    /// NOTE: if the client would call a method in host which would operate host's local vars, you have to add "MarshalByRefObject" for host class like below.
    /// <para>And only non-static vars can be operate. Statics are always native.</para>
    /// </summary>
    class Program : MarshalByRefObject
    {
        /// <summary>
        /// This var is changed indirectly by client
        /// </summary>
        public int TestChamber = 0;

        /// <summary>
        /// This method is called by client
        /// </summary>
        /// <param name="subjectNameHere"></param>
        public void YouSavedScience(object subjectNameHere)
        {
            MessageBox.Show("[Host]Got a message from client:\n" + subjectNameHere.ToString(),
                   Process.GetCurrentProcess().ProcessName);
            TestChamber++;
        }

        /// <summary>
        /// You can't register a method like <see cref="YouSavedScience"/> in static methods.
        /// </summary>
        public void WakingUpToScience()
        {
            int pid = 0;
            Console.WriteLine("Input process name:");
            string processName = Console.ReadLine();
            //Get a process by name
            var ps = Process.GetProcessesByName(processName);
            bool getted = false;
            foreach (var process in ps)
            {
                if (string.IsNullOrEmpty(process.MainWindowTitle))
                    continue;
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
            //pass through target PID
            InjectableProcess ip = new InjectableProcess(pid);

            //Good morning. You have been in suspension for nine nine nine... nine nine ni- 
            ip.SleepInterval = 9999999; //Don't worry, since when we call Eject, the dll thread will be woke up immediately.

            //Register a method to handle DLL's response
            //Always register methods BEFORE DLL injection
            ip.OnClientResponse += YouSavedScience;
            //If a method would not associate with any local vars (like below), it is safe and can be registered even in static methods
            ip.OnClientExit += () => { MessageBox.Show("[Host]Got client offline message.\nNow I only Want You Gone-"); };
            
            //Inject method would return 0 If inject failed (same as VInjDn do)
            if (ip.Inject(@"TestDLL.dll") == 0)
            {
                Console.WriteLine("Failed to inject!");
                Console.ReadLine();
                return;
            }
            //Commands To Test By
            ip.Command("This was a triumph.");
            Console.ReadLine();
            //Reconstructing More Science
            ip.Command("VinjEx by Ulysses - wdwxy12345@gmail.com");
            Console.ReadLine();
            //Use this to release DLL
            ip.Eject();
            Console.WriteLine("Total Response:" + TestChamber);
            Console.ReadLine();
        }

        private static void Main(string[] args)
        {
            Program p = new Program();
            p.WakingUpToScience();
        }
    }

}
