using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace VinjEx
{
    public delegate void CommandHandler(object command);
    public delegate void ExitHandler();

    public class InjectInterface : MarshalByRefObject
    {
        public event CommandHandler OnCommand;
        public event CommandHandler OnResponse;
        public event ExitHandler OnExit;
        public object Data = null;
        public EventWrapper Wrapper;
        public bool ShouldExit = false;
        public bool Connected { get; private set; } = false;

        public InjectInterface()
        {
        }

        public override object InitializeLifetimeService()
        {
            //return base.InitializeLifetimeService();
            return null;
        }

        public bool SendCommand(object command)
        {
            if (OnCommand == null) return false;
            OnCommand(command);
            return true;
        }

        public bool SendResponse(object response)
        {
            if (OnResponse == null) return false;
            OnResponse(response);
            return true;
        }

        public void Ping()
        {
            Connected = true;
            //MessageBox.Show("Ping from" + Process.GetCurrentProcess().MainWindowTitle);
        }

        public void Destory()
        {
            ShouldExit = true;
        }
    }
}
