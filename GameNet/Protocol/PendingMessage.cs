namespace GameNet.Protocol
{
    public class PendingMessage
    {
        public IRecipient Recipient { get; }
        public byte[] Data { get; }

        public PendingMessage(IRecipient recipient, byte[] data)
        {
            Recipient = recipient;
            Data = data;
        }
    }
}