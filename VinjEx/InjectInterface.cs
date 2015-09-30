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
            if (OnCommand != null)
            {
                OnCommand(command);
                return true;
            }
            return false;
        }

        public bool SendResponse(object response)
        {
            if (OnResponse != null)
            {
                OnResponse(response);
                return true;
            }
            return false;
        }

        public void Ping()
        {
            //MessageBox.Show("Ping from" + Process.GetCurrentProcess().MainWindowTitle);
        }

        public void Destory()
        {
            ShouldExit = true;
            OnExit?.Invoke();
        }
    }
}
