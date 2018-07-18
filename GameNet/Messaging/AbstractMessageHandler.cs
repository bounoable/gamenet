using System.Linq;

namespace GameNet.Messaging
{
    abstract public class AbstractMessageHandler<T>: IMessageHandler
    {
        RecipientType handledRecipients;

        /// <summary>
        /// Initialize the message handler.
        /// </summary>
        /// <param name="handledRecipients">The recipient types that should be handled.</param>
        public AbstractMessageHandler(RecipientType handledRecipients)
        {
            this.handledRecipients = handledRecipients;
        }

        /// <summary>
        /// Handle a received message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="recipient">The recipient type.</param>
        public void Handle(IMessage message, RecipientType recipient)
        {
            if (!HandlesRecipient(recipient)) {
                return;
            }

            byte[] data = message.Data
                .Skip(sizeof(int))
                .Take(message.Data.Length - sizeof(int))
                .ToArray();

            HandleObject(ParseObject(data));
        }

        /// <summary>
        /// Determine if a recipient type should be handled.
        /// </summary>
        /// <param name="recipient"></param>
        public bool HandlesRecipient(RecipientType recipient) => (handledRecipients & recipient) == recipient;

        /// <summary>
        /// Reconstruct the object from the received bytes.
        /// </summary>
        /// <param name="data">The object as a byte array.</param>
        /// <returns>The reconstructed object.</returns>
        abstract protected T ParseObject(byte[] data);

        /// <summary>
        /// Handle a parsed object.
        /// </summary>
        /// <param name="parsed">The parsed object.</param>
        abstract protected void HandleObject(T parsed);
    }
}