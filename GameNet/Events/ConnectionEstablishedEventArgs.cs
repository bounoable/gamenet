using System;

namespace GameNet.Events
{
    public class ConnectionEstablishedEventArgs: EventArgs
    {
        public DateTime EstablishedAt { get; } = DateTime.Now;
    }
}