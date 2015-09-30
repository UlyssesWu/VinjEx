using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EasyHook;

namespace VinjEx
{
    internal class EntryPoint
    {
        private readonly InjectInterface _interface;
        internal readonly string ChannelName;
        private bool _shouldExit = false;
        internal EntryPoint(RemoteHooking.IContext inContext, String inChannelName)
        {
            ChannelName = inChannelName;
            _interface = RemoteHooking.IpcConnectClient<InjectInterface>(ChannelName);
        }

        internal void Run(object inContext, String inChannelName)
        {
            _interface.OnExit += () => { _shouldExit = true; };
            _interface.OnCommand += OnCommand;
            OnLoad();
            RemoteHooking.WakeUpProcess();
            while (!_shouldExit)
            {
                //Console.ReadLine();
            }
            OnUnload();
        }
    }
}
