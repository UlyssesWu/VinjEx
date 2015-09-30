using System;
using System.Collections;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Serialization.Formatters;
using EasyHook;

namespace VinjEx
{
    /// <summary>
    /// Injection Entry
    /// <para>You should create such a constructor:</para>
    /// <example>public <see cref=".ctor"/>(<see cref="RemoteHooking.IContext"></see> inContext, <see cref="String"></see> channelName) : base(inContext, channelName)</example>
    /// </summary>
    public abstract class Injectable : MarshalByRefObject, IEntryPoint
    {
        private InjectInterface _interface;
        private readonly string ChannelName;
        private bool _shouldExit = false;
        private static IpcServerChannel _channel;
        
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
            props["portName"] = Guid.NewGuid().ToString();
            BinaryServerFormatterSinkProvider serverProvider = new BinaryServerFormatterSinkProvider(props,null);
            BinaryClientFormatterSinkProvider clientProvider = new BinaryClientFormatterSinkProvider();
            serverProvider.TypeFilterLevel = TypeFilterLevel.Full;

            _channel = new IpcServerChannel(props, serverProvider);
            ChannelServices.RegisterChannel(_channel,false);

            InjectInterface Interface =
                (InjectInterface) Activator.GetObject(typeof (InjectInterface), "ipc://" + channel + "/" + channel);

            if (Interface == null)
                throw new ArgumentException("Unable to create remote interface.");

            return Interface;
        }

        public Injectable(RemoteHooking.IContext inContext, String inChannelName)
        {
            ChannelName = inChannelName;

            _interface = IpcConnectClient(ChannelName);
            _interface.Ping();
            _interface.Wrapper = new EventWrapper();
            _interface.Wrapper.OnCommand += OnCommand;
            _interface.OnResponse += _interface.Wrapper.FireResponse;
            //_interface.TestString = "Hello Moto";
        }

        public void Run(object inContext, String inChannelName)
        {
            OnLoad();
            RemoteHooking.WakeUpProcess();
            //_interface.OnExit += Exit;
            while (!_interface.ShouldExit)
            {
                //Thread.Sleep(10); //Would it be more efficient?
            }
            OnUnload();
        }

        /// <summary>
        ///  Stop the Inject DLL. Will call OnUnload
        /// </summary>
        protected void Exit()
        {
            _shouldExit = true;
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
            _interface.SendResponse(command);
        }

        /// <summary>
        /// Called when the inject dll is going to exit.
        /// </summary>
        public virtual void OnUnload()
        {
        }


    }
}
