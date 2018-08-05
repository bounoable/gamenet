using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace GameNet.Messaging
{
    public class MessageParser: IDataHandler
    {
        MessageTypeConfig typeConfig;
        RecipientType recipient;

        /// <summary>
        /// Initialize a message parser.
        /// </summary>
        /// <param name="typeConfig">The message type config.</param>
        /// <param name="recipient">The recipient type.</param>
        public MessageParser(MessageTypeConfig typeConfig, RecipientType recipient)
        {
            this.typeConfig = typeConfig;
            this.recipient = recipient;
        }

        /// <summary>
        /// Handle received data, parse the message and pass it to the registered handlers.
        /// </summary>
        /// <param name="data">The received data.</param>
        public void Handle(byte[] data)
        {
            IPacket packet = ParsePacket(data);
            IMessageType type = typeConfig.GetTypeById(packet.MessageTypeId);

            if (type == null) {
                return;
            }

            object obj = type.Serializer.Deserialize(data.Skip(sizeof(int)).ToArray());

            type.Handler.Handle(obj, recipient);
        }

        /// <summary>
        /// Parse the message from received data.
        /// </summary>
        /// <param name="raw">The received data.</param>
        /// <returns>The parsed message.</returns>
        IPacket ParsePacket(byte[] raw)
        {
            int type = DataHelper.GetInt(raw);
            byte[] data = raw.Skip(sizeof(int)).ToArray();

            return new Packet(type, data);
        }
    }
}