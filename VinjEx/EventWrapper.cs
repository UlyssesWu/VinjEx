using System;

namespace VinjEx
{
    /// <summary>
    /// EventWrapper used for two-way communication.
    /// No need to use it manually.
    /// </summary>
    public sealed class EventWrapper : MarshalByRefObject
    {
        public event CommandHandler OnCommand;
        public event CommandHandler OnResponse;
        public event ExitHandler OnExit;

        public void FireCommand(object command)
        {
            OnCommand?.Invoke(command);
        }

        public void FireResponse(object response)
        {
            OnResponse?.Invoke(response);
        }

        public void FireExit()
        {
            OnExit?.Invoke();
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
