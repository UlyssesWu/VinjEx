using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using EasyHook;
using Timer = System.Threading.Timer;

namespace TestDLL
{
    public class Main2 : VinjEx.Injectable
    {
        public int CooperationPoints = 0;
        //To test if dll can run after injector exited
        //private Timer _timer = new Timer(Tick,null,3000,10000);

        //private static void Tick(object state)
        //{
        //    MessageBox.Show("Tick");
        //}

        public Main2(RemoteHooking.IContext inContext, string channel) : base(inContext, channel)
        {
        }
        
        public override void OnLoad()
        {
            MessageBox.Show("[Client]DLL Injected\nOh, it's you!");
        }

        public override void OnCommand(object command)
        {
            CooperationPoints++;
            Process p = Process.GetCurrentProcess();

            if (command is string)
            {
                MessageBox.Show("[Client]Got a message from host:\n" + (string)command,p.ProcessName);
                SendResponse("I'm making a note here: HUGE SUCCESS");
            }
            else if(command is int)
            {
                StringBuilder reconstructor = new StringBuilder();
                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    reconstructor.AppendLine(assembly.FullName);
                }
                MessageBox.Show("[Client]I'm a spy!\n", p.ProcessName);
                MessageBox.Show(reconstructor.ToString(), AppDomain.CurrentDomain.FriendlyName);

                reconstructor.Clear();
                reconstructor.AppendLine("FileName:\t" + p.MainModule.FileName);
                reconstructor.AppendLine("Version:\t\n" + p.MainModule.FileVersionInfo);
                reconstructor.AppendLine("ID:\t" + p.Id);
                reconstructor.AppendLine("RAM:\t" + p.PagedSystemMemorySize64);
                SendResponse(reconstructor.ToString());
            }
        }

        public override void OnUnload()
        {
            MessageBox.Show("[Client]DLL Ejected\nAnd when you're dead I will be Still Alive...\nTotal Command:"+CooperationPoints.ToString());
            base.OnUnload();
        }

        //public void GetNames()
        //{
        //    string lastname = "";
        //    string name;
        //    while (true)
        //    {
        //        name = Process.GetCurrentProcess().MainWindowTitle;
        //        if (!string.IsNullOrEmpty(name) && name != lastname)
        //        {
        //            Console.WriteLine(name);
        //            SendResponse(name);
        //            lastname = name;
        //        }
        //        Thread.Sleep(1000);
        //    }
        //}
    }
}
