using System;

namespace GameNet.Messages
{
    public class SystemMessage<T>: EnumAckMessage<T> where T: Enum
    {
        public T Type => Value;

        public SystemMessage(T type, string ackToken = null): base(type, ackToken)
        {}
    }
}