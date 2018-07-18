namespace GameNet.Debug
{
    public interface IClientDebugger
    {
        /// <summary>
        /// Handle a network event.
        /// </summary>
        /// <param name="ev">The event type.</param>
        /// <param name="data">The event data.</param>
        void Handle(ClientEvent ev, EventData data = null);
    }
}