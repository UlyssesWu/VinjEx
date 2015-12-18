using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels.Ipc;
using EasyHook;

namespace VinjEx
{
    /// <summary>
    /// Injectable Process
    /// </summary>
    public class InjectableProcess
    {
        /// <summary>
        /// default thread sleep time
        /// </summary>
        public const int SLEEP_TIME = 1000;

        private readonly int _pid;
        private readonly string _channelName;
        private InjectInterface _interface;
        //Although we never use it, it should be kept until you finish this dll injection.
        private static IpcChannel _channel;
        /// <summary>
        /// Register by host. Fired when client send response.
        /// </summary>
        public event CommandHandler OnClientResponse;
        /// <summary>
        /// Register by host. Fired after client unload.
        /// </summary>
        public event EventHandler OnClientExit;

        /// <summary>
        /// How much time(ms) dll thread will sleep once when idle.
        /// Will pass to dll thread when call <see cref="Inject"/>. Would be useless after that.
        /// </summary>
        public int SleepInterval
        {
            get
            {
                return _interface?.SleepInterval ?? SLEEP_TIME;
            }
            set
            {
                if (_interface != null)
                {
                    _interface.SleepInterval = value;
                }
            }
        }

        public bool IsBackgroundThread = true;

        internal event CommandHandler OnHostCommand;

        /// <summary>
        /// Injectable Process
        /// </summary>
        /// <param name="pid">target PID</param>
        /// <param name="sleepInterval">how much time dll thread will sleep once when idle</param>
        public InjectableProcess(int pid, int sleepInterval = SLEEP_TIME)
        {
            _pid = pid;
            _interface = new InjectInterface();
            SleepInterval = sleepInterval;
            //MARK:An IpcChannel that shall be keept alive until the server is not needed anymore.
            _channel = Util.IpcCreateServer(ref _channelName, WellKnownObjectMode.Singleton, _interface);//MARK:注意第三个参数
        }

        /// <summary>
        /// [For Compatibility] Create a <see cref="InjectableProcess"/>.
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        public static InjectableProcess Create(int pid)
        {
            InjectableProcess ip = new InjectableProcess(pid);
            return ip;
        }

        private void RegisterEvents()
        {
            try
            {
                //Methods that will be called by host
                _interface.Wrapper.OnResponse += OnClientResponse;
                _interface.Wrapper.OnExit += OnClientExit;
                //Methods that will be called by client
                OnHostCommand += _interface.Wrapper.FireCommand;
            }
            catch (Exception ex)
            {
                throw new Exception("[VinjEx] Error when trying to register events.", ex);
            }
        }

        /// <summary>
        /// Send command to Injection DLL
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        public bool Command(object command)
        {
            if (OnHostCommand != null)
            {
                try
                {
                    OnHostCommand(command);
                    return true;
                }
                catch (RemotingException)
                {
                    return false;
                }

            }
            return false;
        }

        /// <summary>
        /// Inject a DLL to target
        /// </summary>
        /// <param name="assemblyFile">x86 DLL</param>
        /// <param name="assemblyFile64">x64 DLL, if your target is 64bit program</param>
        /// <returns></returns>
        public int Inject(string assemblyFile, string assemblyFile64 = null)
        {
            try
            {
                _interface.SleepInterval = SleepInterval;
                _interface.IsBackgroundThread = IsBackgroundThread;
                if (RemoteHooking.IsX64Process(_pid))
                {
                    //Console.WriteLine("64bit program!");
                }
                RemoteHooking.Inject(_pid, assemblyFile, assemblyFile64, _channelName);

                RegisterEvents();
                return _pid;
            }
            catch (Exception)
            {
                //FIXED: The Part Where He Kills You
                Eject();
                return 0;
            }
        }

        /// <summary>
        /// Eject the DLL
        /// </summary>
        public void Eject()
        {
            _interface?.Destory();
        }
    }
}
