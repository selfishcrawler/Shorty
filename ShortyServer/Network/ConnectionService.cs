using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ShortyServer.Configuration;
using System.Net;
using System.Net.Sockets;

namespace ShortyServer.Network;

internal class ConnectionService : IConnectionService
{
    private readonly ILogger<IConnectionService> logger;
    private readonly NetworkClientFactory factory;
    private readonly IPAddress ip;
    private readonly int port;
    private readonly TcpListener listener;
    private readonly List<INetworkClient> clients;

    private CancellationTokenSource cts;

    public bool Running => cts is not null && !cts.IsCancellationRequested;

    public ConnectionService(ILogger<IConnectionService> logger,
        IOptions<NetworkConnectionOptions> connectionOptions,
        NetworkClientFactory factory)
    {
        this.logger = logger;
        this.factory = factory;
        if (!IPAddress.TryParse(connectionOptions.Value.IP, out ip))
        {
            throw new ArgumentException("Cannot resolve IP address from config", nameof(ip));
        }
        port = connectionOptions.Value.Port;

        if (port < 0 || port > ushort.MaxValue - 1)
        {
            throw new ArgumentException("Invalid port range", nameof(port));
        }

        listener = new TcpListener(ip, port);
        logger.LogInformation("Initialized with IP: {ip}, Port: {port}", ip, port);
    }

    public void Start()
    {
        cts = new CancellationTokenSource();
        try
        {
            listener.Start();
            logger.LogInformation("Connection service started");
        }
        catch (SocketException se)
        {
            logger.LogCritical(se, "Error while starting up listener");
            throw;
        }
    }
    public async Task HandleIncomingConnections()
    {
        while (!cts.IsCancellationRequested)
        {
            var tcpClient = await listener.AcceptTcpClientAsync(cts.Token);
            var networkClient = factory.Create(tcpClient);
            clients.Add(networkClient);
            _ = networkClient.HandleConnection().ContinueWith(t =>
            {
                if (t.IsFaulted)
                {
                    // Handle client exception here...
                }
            });
        }
    }

    public void Stop()
    {
        cts.Cancel();
        listener.Stop();
        logger.LogInformation("Connection service stopped");
    }
}
