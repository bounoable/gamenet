using System;

namespace GameNet.Messages
{
    public class MessageActionHandler<T>: MessageHandler<T> where T: class
    {
        readonly Action<T> _handler;

        public MessageActionHandler(Action<T> handler)
        {
            _handler = handler;
        }

        override protected void HandleMessage(T message)
        {
            if (_handler != null) {
                _handler(message);
            }
        }
    }
}