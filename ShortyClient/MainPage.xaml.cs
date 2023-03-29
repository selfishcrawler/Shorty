using System.Net;
using System.Net.Sockets;

namespace ShortyClient;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        //just for test by now
        var client = new TcpClient();
        client.Connect(IPAddress.Loopback, 3928);
    }
}