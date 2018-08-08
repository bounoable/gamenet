namespace GameNet.Messages
{
    public interface IMessageHandler
    {
        /// <summary>
        /// Handle a deserialized object .
        /// </summary>
        /// <param name="message">The deserialized object.</param>
        void Handle(object message);
    }
}