namespace GameNet.Messages
{
    public interface IAcknowledgeMessage
    {
        /// <summary>
        /// The unique acknowledge token of the message.
        /// </summary>
        byte[] AckToken { get; }
    }
}