using System;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;
using EasyHook;

namespace TestDLL
{
    public class Main2 : VinjEx.Injectable
    {
        public int CooperationPoints = 0;
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
                reconstructor.AppendLine("FileName:\t" + p.MainModule.FileName);
                reconstructor.AppendLine("Version:\t\n" + p.MainModule.FileVersionInfo);
                reconstructor.AppendLine("ID:\t" + p.Id);
                reconstructor.AppendLine("RAM:\t" + p.PagedSystemMemorySize64);
                MessageBox.Show("[Client]I'm a spy!\n", p.ProcessName);
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
