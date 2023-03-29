namespace ShortyServer.Network;

internal interface INetworkClient
{
    public Task HandleConnection();
}
