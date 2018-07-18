namespace GameNet.Debug
{
    public interface IServerDebugger
    {
        /// <summary>
        /// Handle a network event.
        /// </summary>
        /// <param name="ev">The event type.</param>
        /// <param name="data">The event data.</param>
        void Handle(ServerEvent ev, EventData data = null);
    }
}