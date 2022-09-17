

using Microsoft.AspNetCore.SignalR.Client;

namespace ChatClient.Hubs;

public class ChatConnection
{
    private readonly HubConnection _connection;

    public ChatConnection()
    {
        _connection = new HubConnectionBuilder()
            
            .WithAutomaticReconnect()
            // .WithUrl("https://localhost:5001/chatHub")
            .Build();
        
        _connection.On<string>("MessageReceived", (message) =>
        {
        });

        Task.Run(() => Dispatcher
            .GetForCurrentThread()
            .Dispatch(async () => await _connection.StartAsync()));
    }
}
