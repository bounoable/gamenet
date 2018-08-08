namespace GameNet.Messages
{
    public class ClientSecretMessage
    {
        public string Secret { get; }

        public ClientSecretMessage(string secret)
            => Secret = secret;
    }
}