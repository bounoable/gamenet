using System;
using System.Linq;
using System.Text;

namespace GameNet.Messaging
{
    abstract public class MessageHandler<T>: IMessageHandler<T>
    {
        RecipientType handledRecipients;

        /// <summary>
        /// Initialize the message handler.
        /// </summary>
        /// <param name="handledRecipients">The recipient types that should be handled.</param>
        public MessageHandler(RecipientType handledRecipients)
        {
            this.handledRecipients = handledRecipients;
        }

        /// <summary>
        /// Handle a received message.
        /// </summary>
        /// <param name="message">The received message.</param>
        /// <param name="recipient">The recipient type.</param>
        public void Handle(T message, RecipientType recipient)
        {
            if (!HandlesRecipient(recipient)) {
                return;
            }

            if (message == null) {
                return;
            }

            HandleMessage(message);
        }

        /// <summary>
        /// Determine if a recipient type should be handled.
        /// </summary>
        /// <param name="recipient"></param>
        public bool HandlesRecipient(RecipientType recipient) => (handledRecipients & recipient) == recipient;

        /// <summary>
        /// Handle a deserialized object .
        /// </summary>
        /// <param name="message">The deserialized object.</param>
        /// <param name="recipient">The recipient type (Server or Client).</param>
        abstract protected void HandleMessage(T message);
    }
}