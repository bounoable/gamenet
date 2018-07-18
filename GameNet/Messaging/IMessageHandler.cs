namespace GameNet.Messaging
{
    public interface IMessageHandler
    {
        /// <summary>
        /// Handle a received message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="recipient">The recipient type (Server or Client).</param>
        void Handle(IMessage message, RecipientType recipient);
    }
}