using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShortyServer.Configuration;
using ShortyServer.Network;
using System.Net.Sockets;

namespace ShortyServer;

internal class Program
{
    static void Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .AddJsonFile(args.Length is 0 ? "appsettings.json" : args[0])
            .Build();

        var services = new ServiceCollection()
            .AddLogging(logger => logger.AddConsole())
            .AddOptions()
            .Configure<NetworkConnectionOptions>(config.GetRequiredSection(nameof(NetworkConnectionOptions)))
            .AddSingleton<IConnectionService, ConnectionService>()
            .AddSingleton<NetworkClientFactory>();

        var serviceProvider = services.BuildServiceProvider();
        var logger = serviceProvider.GetService<ILoggerFactory>()
            .CreateLogger<Program>();

        try
        {
            var connectionService = serviceProvider.GetRequiredService<IConnectionService>();
            connectionService.Start();
            _ = connectionService.HandleIncomingConnections();
        }
        catch (ArgumentException ae)
        {
            logger.LogCritical(ae, "Error during connection service initialization");
            return;
        }
        catch (SocketException)
        {
            return;
        }

        while (true)
        {
            string cmd = Console.ReadLine();
        }
    }
}