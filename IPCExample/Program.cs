using System;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting;
using System.Reflection;
using System.Collections;

/*
This Example is provided by Sowmy Srinivasan.
URL: https://social.msdn.microsoft.com/Forums/en-US/b0b75d32-9ed8-404c-b8f3-d3cb4c3d241f/ipc-remoting-exception-with-events?forum=netfxremoting
*/

namespace IpcCallbackSample
{
    class Program : MarshalByRefObject
    {
        public void CallServer(Program client, string sender)
        {
            Console.WriteLine("Message from {0}", sender);
            client.CallbackClient("Server");
        }

        public void CallbackClient(string sender)
        {
            Console.WriteLine("Message from {0}", sender);
        }

        static void Main(string[] args)
        {
            if (args == null || args.Length == 0 || args[0].ToLower() == "-server")
            {
                RegisterChannel("Server");
                RemotingConfiguration.RegisterWellKnownServiceType(typeof(Program), "RemotingServer", WellKnownObjectMode.SingleCall);
                Console.WriteLine("Server Running. Start client by typing '{0} -client'", Assembly.GetEntryAssembly().Location);
                Console.ReadLine();
            }
            else
            {
                RegisterChannel("Client");
                Program proxy = (Program)Activator.GetObject(typeof(Program), "ipc://Server/RemotingServer");
                proxy.CallServer(new Program(), "Client");
            }
        }

        static void RegisterChannel(string name)
        {
            Hashtable properties = new Hashtable();
            properties.Add("name", name);
            properties.Add("portName", name);
            properties.Add("typeFilterLevel", "Full");
            IpcChannel channel = new IpcChannel(properties,
                new BinaryClientFormatterSinkProvider(properties, null),
                new BinaryServerFormatterSinkProvider(properties, null));
            ChannelServices.RegisterChannel(channel,false);
        }
    }
}
