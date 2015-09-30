using System.Diagnostics;
using System.Windows.Forms;
using EasyHook;

namespace TestDLL
{
    public class Main2 : VinjEx.Injectable
    {
        public Main2(RemoteHooking.IContext inContext, string channel) : base(inContext, channel)
        {
        }

        public override void OnLoad()
        {
            MessageBox.Show("DLL Injected\nOh, it's you!");
        }

        public override void OnCommand(object command)
        {
            MessageBox.Show("[Client]Got a message from host:\n"+(string)command, Process.GetCurrentProcess().ProcessName);
            SendResponse("I'm making a note here: HUGE SUCCESS");
        }

        public override void OnUnload()
        {
            MessageBox.Show("DLL Ejected\nAnd when you're dead I will be still alive...");
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
