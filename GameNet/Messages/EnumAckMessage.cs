using System;

namespace GameNet.Messages
{
    public class EnumAckMessage<T>: AcknowledgeRequest where T: Enum
    {
        public T Value { get; }

        public EnumAckMessage(T value, string ackToken = null): base(ackToken)
        {
            Value = value;
        }
    }
}