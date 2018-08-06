using System;

namespace GameNet.Messages
{
    public class SystemMessage: AcknowledgeMessage
    {
        public enum MessageType
        {
            StillConnected
        }

        public MessageType Type { get; }

        public SystemMessage(MessageType type): base()
        {
            Type = type;
        }

        public SystemMessage(MessageType type, byte[] ackToken): base(ackToken)
        {
            Type = type;
        }
    }
}