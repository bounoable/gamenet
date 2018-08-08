using System;
using GameNet.Messages;

namespace GameNet.Protocol
{
    public class PendingAcknowledgeRequest
    {
        public IAcknowledgeRequest Message { get; }
        public IRecipient Recipient { get; }
        public int Tries { get; set; } = 0;
        public DateTime LastTry { get; set; }

        public PendingAcknowledgeRequest(IAcknowledgeRequest message, IRecipient recipient)
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