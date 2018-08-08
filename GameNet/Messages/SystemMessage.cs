using System;

namespace GameNet.Messages
{
    public class SystemMessage<T>: AcknowledgeMessage where T: Enum
    {
        public T Type { get; }

        public SystemMessage(T type): base()
        {
            Type = type;
        }

        public SystemMessage(T type, byte[] ackToken): base(ackToken)
        {
            Type = type;
        }
    }
}