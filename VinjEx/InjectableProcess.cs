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
        private int _pid = 0;
        private readonly string _channelName;
        private InjectInterface _interface;
        private static IpcChannel _channel;
        public event CommandHandler OnClientResponse;
        public event CommandHandler OnServerCommand;
        /// <summary>
        /// Injectable Process
        /// </summary>
        /// <param name="pid">target PID</param>
        public InjectableProcess(int pid)
        {
            _pid = pid;
            _interface = new InjectInterface();
            //MARK:An IpcChannel that shall be keept alive until the server is not needed anymore.
            _channel = Util.IpcCreateServer<InjectInterface>(ref _channelName, WellKnownObjectMode.Singleton, _interface);//MARK:注意第三个参数
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
                _interface.Wrapper.OnResponse += OnClientResponse;
                OnServerCommand += _interface.Wrapper.FireServerCommand;
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
        /// <returns>命令是否成功发送</returns>
        public bool Command(object command)
        {
            if (OnServerCommand != null)
            {
                OnServerCommand(command);
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
            catch (Exception ex)
            {
                return 0;
                throw;
            }
        }

        public void Eject()
        {
            if (_interface != null)
            {
                _interface.Destory();
            }
        }
    }
}
