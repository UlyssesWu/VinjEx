using System;
using System.Diagnostics;
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
            MessageBox.Show("[Client]Got a message from host:\n"+(string)command, Process.GetCurrentProcess().ProcessName);
            SendResponse("I'm making a note here: HUGE SUCCESS");
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
