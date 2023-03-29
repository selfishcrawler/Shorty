namespace ShortyServer.Network;

internal interface IConnectionService
{
    public bool Running { get; }

    public void Start();
    public void Stop();
    public Task HandleIncomingConnections();
}
