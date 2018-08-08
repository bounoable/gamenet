using System;

namespace GameNet.Messages
{
    public class SystemMessage<T>: AcknowledgeRequest where T: Enum
    {
        public T Type { get; }

        public SystemMessage(T type): base()
        {
            Type = type;
        }

        public SystemMessage(T type, string ackToken): base(ackToken)
        {
            Type = type;
        }
    }
}