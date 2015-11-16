using System;

namespace VinjEx
{

    internal class InjectInterface : MarshalByRefObject
    {
        public event CommandHandler OnCommand;
        public event CommandHandler OnResponse;
        public event EventHandler OnExit;
        /// <summary>
        /// used to stop dll thread
        /// </summary>
        public event EventHandler OnClientExit;
        public object Data = null;
        public EventWrapper Wrapper;
        public bool IsBackgroundThread = true;
        public int SleepInterval = InjectableProcess.SLEEP_TIME;
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
            try
            {
                OnClientExit?.Invoke(null, null); //May not execute in some force close condition?
            }
            catch (Exception)
            {
                //throw;
            }
        }
    }
}
