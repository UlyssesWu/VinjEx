using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using VinjEx;

namespace InjectTest
{
    class Main : Injectable
    {
        public override void OnLoad()
        {
            base.OnLoad();
            Thread th = new Thread(GetNames);
            th.Start();
        }

        public override void OnCommand(object command)
        {
            MessageBox.Show((string) command);
        }

        public override void OnUnload()
        {
            MessageBox.Show("Ejected!");
            base.OnUnload();
        }

        public void GetNames()
        {
            string lastname = "";
            string name;
            while (true)
            {
                name = Process.GetCurrentProcess().MainWindowTitle;
                if (!string.IsNullOrEmpty(name) && name != lastname)
                {
                    SendResponse(name);
                    lastname = name;
                }
                Thread.Sleep(1000);
            }
        }
    }
}
