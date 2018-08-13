using System;

namespace GameNet.Messages
{
    public class EnumMessage<T> where T: Enum
    {
        public T Value { get; }

        public EnumMessage(T value)
        {
            Value = value;
        }
    }
}