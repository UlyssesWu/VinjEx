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
        private readonly int _pid;
        private readonly string _channelName;
        private InjectInterface _interface;
        private static IpcChannel _channel;
        /// <summary>
        /// Register by host. Fired when client send response.
        /// </summary>
        public event CommandHandler OnClientResponse;
        /// <summary>
        /// Register by host. Fired after client unload.
        /// </summary>
        public event ExitHandler OnClientExit;
        
        internal event CommandHandler OnHostCommand;
        /// <summary>
        /// Injectable Process
        /// </summary>
        /// <param name="pid">target PID</param>
        public InjectableProcess(int pid)
        {
            _pid = pid;
            _interface = new InjectInterface();
            //MARK:An IpcChannel that shall be keept alive until the server is not needed anymore.
            _channel = Util.IpcCreateServer(ref _channelName, WellKnownObjectMode.Singleton, _interface);//MARK:注意第三个参数
            //var _channel = RemoteHooking.IpcCreateServer<InjectInterface>(ref _channelName, WellKnownObjectMode.SingleCall);//MARK:注意第三个参数
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
                throw new Exception("[VinjEx] Error when trying to register events.",ex);
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
                OnHostCommand(command);
                return true;
            }
            else
            {
                return false;
            }

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
                RemoteHooking.Inject(_pid,assemblyFile,string.IsNullOrEmpty(assemblyFile64)?assemblyFile:assemblyFile64,_channelName);

                RegisterEvents();
                return _pid;
            }
            catch (Exception)
            {
                return 0;
                //throw;
            }
        }

        /// <summary>
        /// Eject the DLL
        /// </summary>
        public void Eject()
        {
            if (_interface != null)
            {
                _interface.Destory();
            }
        }
    }
}
