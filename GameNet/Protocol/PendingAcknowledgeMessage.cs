using System;
using GameNet.Messages;

namespace GameNet.Protocol
{
    public class PendingAcknowledgeMessage
    {
        public IAcknowledgeMessage Message { get; }
        public IRecipient Recipient { get; }
        public int Tries { get; set; } = 0;

        public PendingAcknowledgeMessage(IAcknowledgeMessage message, IRecipient recipient)
        {
            if (message == null) {
                throw new ArgumentNullException("message");
            }

            if (recipient == null) {
                throw new ArgumentNullException("recipient");
            }

            Message = message;
            Recipient = recipient;
        }
    }
}