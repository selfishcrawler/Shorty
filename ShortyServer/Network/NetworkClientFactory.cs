using Microsoft.Extensions.Logging;
using System.Net.Sockets;

namespace ShortyServer.Network;

internal class NetworkClientFactory
{
    private readonly ILogger<INetworkClient> logger;

    public NetworkClientFactory(ILogger<INetworkClient> logger)
    {
        this.logger = logger;
    }

    public INetworkClient Create(TcpClient tcpClient) => new NetworkClient(logger, tcpClient);
}
