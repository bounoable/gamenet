namespace GameNet.Messaging
{
    public interface IMessageHandler
    {
        /// <summary>
        /// Handle a deserialized object .
        /// </summary>
        /// <param name="message">The deserialized object.</param>
        /// <param name="recipient">The recipient type (Server or Client).</param>
        void Handle(object message, RecipientType recipient);
    }
}