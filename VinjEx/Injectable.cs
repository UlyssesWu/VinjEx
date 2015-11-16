using System;
using System.Collections;
using System.Diagnostics;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using EasyHook;

namespace VinjEx
{
    /// <summary>
    /// Injection Entry
    /// <para>You should create such a constructor:</para>
    /// <example>public ClassNameHere(<see cref="RemoteHooking.IContext"></see> inContext, <see cref="String"></see> channelName) : base(inContext, channelName)</example>
    /// </summary>
    public abstract class Injectable : MarshalByRefObject, IEntryPoint
    {
        public readonly string ChannelName;
        private readonly InjectInterface _interface;
        private bool _shouldExit = false;
        private bool _unloaded = false;
        private static IpcServerChannel _channel;
        private Thread _thread;
        private int _sleepInterval = InjectableProcess.SLEEP_TIME;

        public override object InitializeLifetimeService()
        {
            return null;
        }

        /// <summary>
        /// Get interface object from host.
        /// <para>Fixed method from Easyhook for two-way communication.</para>
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        private InjectInterface IpcConnectClient(string channel)
        {
            IDictionary props = new Hashtable();
            props["name"] = Util.GenerateName();
            props["port"] = 0;
            props["portName"] = props["name"];
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider(props, null);
            //BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;

            _channel = new IpcServerChannel(props, serverProvider);
            ChannelServices.RegisterChannel(_channel, false);

            InjectInterface Interface =
                (InjectInterface)Activator.GetObject(typeof(InjectInterface), "ipc://" + channel + "/" + channel);

            if (Interface == null)
                throw new ArgumentException("Unable to create remote interface.");

            return Interface;
        }

        public Injectable(RemoteHooking.IContext inContext, String inChannelName)
        {
            ChannelName = inChannelName;

            _interface = IpcConnectClient(ChannelName);
            _interface.Ping();

            _sleepInterval = _interface.SleepInterval;
            _interface.Wrapper = new EventWrapper();
            _interface.Wrapper.OnCommand += OnCommand;
            _interface.OnResponse += _interface.Wrapper.FireResponse;
            _interface.OnExit += _interface.Wrapper.FireExit;
        }

        /// In order to save resources, we make it sleep.
        public void Run(object inContext, String inChannelName)
        {
            OnLoad();
            _thread = Thread.CurrentThread;
            AppDomain.CurrentDomain.ProcessExit += (sender, args) =>
            {
                if (!_unloaded)
                {
                    OnUnload();
                    _interface.Wrapper.FireExit(null, null);
                    _unloaded = true;
                }
            };
            _thread.IsBackground = _interface.IsBackgroundThread;
            _interface.OnClientExit += Exit; //Only at this time can we make sure the dll thread is interruptable
            RemoteHooking.WakeUpProcess();
            while (!_shouldExit)
            {
                try
                {
                    Thread.Sleep(_sleepInterval); //Would it be more efficient?
                }
                catch (ThreadInterruptedException)
                {
                    //帅醒！
                }
                catch(RemotingException)
                { }
            }
            if (!_unloaded)
            {
                OnUnload();
                _interface.Wrapper.FireExit(null, null);
                _unloaded = true;
            }
        }

        /// <summary>
        ///  Stop the Inject DLL. Will call OnUnload after Exit
        /// </summary>
        public void Exit(object sender,EventArgs e)
        {
            _shouldExit = true;
            _thread?.Interrupt();
        }

        /// <summary>
        /// Call when the Inject DLL is loaded, before the target remuse.
        /// </summary>
        public virtual void OnLoad()
        {
        }

        /// <summary>
        /// Called when host send a command.
        /// </summary>
        /// <param name="command"></param>
        public virtual void OnCommand(object command)
        {
            Console.WriteLine(command);
        }

        /// <summary>
        /// Send a message to host.
        /// </summary>
        /// <param name="command"></param>
        public void SendResponse(object command)
        {
            try
            {
                _interface.SendResponse(command);
            }
            catch (RemotingException)
            {
                //throw;
            }
            
        }

        /// <summary>
        /// Called when the inject dll is going to exit. 
        /// Will be called after Exit. Be careful if you call Exit manually.
        /// </summary>
        public virtual void OnUnload()
        {
        }


    }
}
