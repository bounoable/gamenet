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
        abstract protected TMessage ParseObject(byte[] data);

        /// <summary>
        /// Handle a parsed object.
        /// </summary>
        /// <param name="parsed">The parsed object.</param>
        abstract protected void HandleObject(TMessage parsed);

        /// <summary>
        /// Slice out a part of bytes from a byte array.
        /// </summary>
        /// <param name="buffer">The byte buffer.</param>
        /// <param name="offset">The number of bytes to skip.</param>
        /// <param name="count">The number of bytes to take.</param>
        /// <returns>The sliced out bytes.</returns>
        protected byte[] Slice(byte[] buffer, int offset, int count)
            => buffer.Skip(offset).Take(count).ToArray();
        
        /// <summary>
        /// Transform bytes into a boolean.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The boolean.</returns>
        protected bool ConvertToBool(byte[] data) => BitConverter.ToBoolean(data, 0);

        /// <summary>
        /// Convert bytes to an int.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The integer.</returns>
        protected int ConvertToInt(byte[] data) => BitConverter.ToInt32(data, 0);

        /// <summary>
        /// Convert bytes to a float.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The float.</returns>
        protected float ConvertToFloat(byte[] data) => BitConverter.ToSingle(data, 0);

        /// <summary>
        /// Convert bytes to a double.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The double.</returns>
        protected double ConvertToDouble(byte[] data) => BitConverter.ToDouble(data, 0);

        /// <summary>
        /// Convert bytes to a char.
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The char.</returns>
        protected char ConvertToChar(byte[] data) => BitConverter.ToChar(data, 0);

        /// <summary>
        /// Convert bytes to a string with UTF-16 encoding..
        /// </summary>
        /// <param name="data">The byte array.</param>
        /// <returns>The string.</returns>
        protected string ConvertToString(byte[] data) => Encoding.Unicode.GetString(data);

        /// <summary>
        /// Get a boolean from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The boolean.</returns>
        protected bool GetBool(byte[] data, int offset = 0)
            => ConvertToBool(Slice(data, offset, sizeof(bool)));
        
        /// <summary>
        /// Get an integer from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The integer.</returns>
        protected int GetInt(byte[] data, int offset = 0)
            => ConvertToInt(Slice(data, offset, sizeof(int)));
        
        /// <summary>
        /// Get a float from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The float.</returns>
        protected float GetFloat(byte[] data, int offset = 0)
            => ConvertToFloat(Slice(data, offset, sizeof(float)));
        
        /// <summary>
        /// Get a double from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The double.</returns>
        protected double GetDouble(byte[] data, int offset = 0)
            => ConvertToFloat(Slice(data, offset, sizeof(double)));
        
        /// <summary>
        /// Get a char from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The char.</returns>
        protected char GetChar(byte[] data, int offset = 0)
            => ConvertToChar(Slice(data, offset, sizeof(char)));
        
        /// <summary>
        /// Get a string from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The string.</returns>
        protected string GetString(byte[] data, int offset = 0)
            => ConvertToString(Slice(data, offset + sizeof(int), GetInt(data, offset) * sizeof(char)));
        
        /// <summary>
        /// Pull a boolean from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The boolean.</returns>
        protected bool PullBool(ref byte[] data)
        {
            bool result = GetBool(data);
            data = data.Skip(sizeof(bool)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull an integer from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The integer.</returns>
        protected int PullInt(ref byte[] data)
        {
            int result = GetInt(data);
            data = data.Skip(sizeof(int)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull a float from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The float.</returns>
        protected float PullFloat(ref byte[] data)
        {
            float result = GetFloat(data);
            data = data.Skip(sizeof(float)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull a double from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The double.</returns>
        protected double GetDouble(ref byte[] data)
        {
            double result = GetDouble(data);
            data = data.Skip(sizeof(double)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull a char from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The char.</returns>
        protected char PullChar(ref byte[] data)
        {
            char result = GetChar(data);
            data = data.Skip(sizeof(char)).ToArray();

            return result;
        }

        /// <summary>
        /// Pull a string from a data buffer.
        /// </summary>
        /// <param name="data">The data buffer.</param>
        /// <param name="offset">The offset.</param>
        /// <returns>The string.</returns>
        protected string PullString(ref byte[] data)
        {
            string result = GetString(data);
            data = data.Skip(result.Length * sizeof(char)).ToArray();

            return result;
        }
    }
}