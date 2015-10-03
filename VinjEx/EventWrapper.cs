using System;

namespace VinjEx
{
    public delegate void CommandHandler(object command);

    /// <summary>
    /// EventWrapper used for two-way communication.
    /// No need to use it manually.
    /// </summary>
    internal sealed class EventWrapper : MarshalByRefObject
    {
        public event CommandHandler OnCommand;
        public event CommandHandler OnResponse;
        public event EventHandler OnExit;

        public void FireCommand(object command)
        {
            OnCommand?.Invoke(command);
        }

        public void FireResponse(object response)
        {
            OnResponse?.Invoke(response);
        }

        public void FireExit(object sender,EventArgs e)
        {
            OnExit?.Invoke(sender,e);
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
