using System;
using System.Linq;
using System.Text;

namespace GameNet.Messaging
{
    abstract public class MessageHandler<TMessage>: IMessageHandler
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
        public void Handle(IMessage message, RecipientType recipient)
        {
            if (!HandlesRecipient(recipient)) {
                return;
            }

            HandleObject(ParseObject(message.Data));
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
        abstract protected TMessage ParseObject(byte[] data);

        /// <summary>
        /// Handle a parsed object.
        /// </summary>
        /// <param name="parsed">The parsed object.</param>
        abstract protected void HandleObject(TMessage parsed);

        /// <summary>
        /// Pull a boolean from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The boolean.</returns>
        protected static bool PullBool(ref byte[] data) => DataHelper.PullBool(ref data);

        /// <summary>
        /// Pull a short (16 bit int) from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The short (16 bit int).</returns>
        protected static short PullShort(ref byte[] data) => DataHelper.PullShort(ref data);

        /// <summary>
        /// Pull an integer from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The integer.</returns>
        protected static int PullInt(ref byte[] data) => DataHelper.PullInt(ref data);

        /// <summary>
        /// Pull a float from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The float.</returns>
        protected static float PullFloat(ref byte[] data) => DataHelper.PullFloat(ref data);

        /// <summary>
        /// Pull a double from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The double.</returns>
        protected static double GetDouble(ref byte[] data) => DataHelper.PullDouble(ref data);

        /// <summary>
        /// Pull a char from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The char.</returns>
        protected static char PullChar(ref byte[] data) => DataHelper.PullChar(ref data);

        /// <summary>
        /// Pull a string from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The string.</returns>
        protected static string PullString(ref byte[] data) => DataHelper.PullString(ref data);
    }
}