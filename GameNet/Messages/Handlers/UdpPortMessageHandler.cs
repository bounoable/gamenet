using GameNet.Protocol;

namespace GameNet.Messages.Handlers
{
    public class UdpPortMessageHandler<TSender>: MessageHandler<UdpPortMessage<TSender>>
    {
        Server _server;
        Client _client;

        public UdpPortMessageHandler(Server server)
            => _server = server;

        public UdpPortMessageHandler(Client client)
            => _client = client;

        override protected void HandleMessage(UdpPortMessage<TSender> message)
        {
            if (_client != null && message is UdpPortMessage<Server>) {
                _client.RegisterServerUdpPort((IUdpPortMessage)message);
            } else if (_server != null && message is UdpPortMessage<Client>) {
                _server.RegisterClientUdpPort((IUdpPortMessage)message);
            }
        }
    }
}