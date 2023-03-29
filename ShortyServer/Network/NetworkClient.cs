using System.Net.Sockets;

namespace ShortyServer.Network;

internal class NetworkClient : INetworkClient
{
    private readonly ILogger<INetworkClient> logger;
    private readonly TcpClient tcpClient;

    public NetworkClient(ILogger<INetworkClient> logger, TcpClient tcpClient)
    {
        this.logger = logger;
        this.tcpClient = tcpClient;
    }

    public async Task HandleConnection()
    {
        logger.LogInformation("Connection successful");
    }
}
