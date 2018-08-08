using System;

namespace GameNet.Messages
{
    public class AcknowledgeMessage: IAcknowledgeMessage
    {
        public static int AckTokenSize
        {
            get => _ackTokenSize;
            set
            {
                if (value > 0) {
                    _ackTokenSize = value;
                }
            }
        }

        static int _ackTokenSize = 2;

        readonly static Random _rand = new Random();

        public byte[] AckToken { get; }

        /// <summary>
        /// Create an acknowledge message.
        /// </summary>
        public AcknowledgeMessage()
        {
            AckToken = new byte[AckTokenSize];
            _rand.NextBytes(AckToken);
        }

        /// <summary>
        /// Create an acknowledge message.
        /// </summary>
        /// <param name="token">The acknowledge token.</param>
        public AcknowledgeMessage(byte[] token)
        {
            AckToken = token;
        }
    }
}