using System;
using System.Linq;
using System.Text;

namespace GameNet.Messages
{
    abstract public class MessageHandler<T>: IMessageHandler where T: class
    {
        /// <summary>
        /// Handle a received message.
        /// </summary>
        /// <param name="message">The received message.</param>
        public void Handle(object message)
        {
            if (message as T == null) {
                return;
            }

            HandleMessage((T)message);
        }

        /// <summary>
        /// Handle a deserialized object .
        /// </summary>
        /// <param name="message">The deserialized object.</param>
        /// <param name="recipient">The recipient type (Server or Client).</param>
        abstract protected void HandleMessage(T message);
    }
}