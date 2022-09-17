using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.SignalR;
using static System.Reactive.Linq.Observable;

namespace ChatClient.Hubs;

public class ChatBridge
{
    private HubConnection _hubConnection;
    private Subject<ChatMessage> _messages = new();

    private readonly object _connectionLock = new();
    private string _connectionPath = "http://localhost:5000/ChatHub";

    public IObservable<ChatMessage> Messages => _messages
        .ObserveOn(Scheduler.Default);


    public async Task Connect()
    {
        try
        {
            lock (_connectionLock)
            {
                SetUpConnection();
            }

            await _hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error on trying to connect to '{_connectionPath}'", ex);
        }
    }

    private void SetUpConnection()
    {
        if (_hubConnection != null && _hubConnection.State != HubConnectionState.Disconnected) return;

        _hubConnection = new HubConnectionBuilder()
            .WithUrl(_connectionPath)
            .Build();

        FromEventPattern<Exception>(
                _ => _hubConnection.Closed += OnConnectionClosed,
                _ => _hubConnection.Closed -= OnConnectionClosed)
            .Subscribe();

        FromEventPattern<Exception>(
                _ => _hubConnection.Reconnecting += OnReconnecting,
                _ => _hubConnection.Reconnecting -= OnReconnecting)
            .Subscribe();

        FromEventPattern<string>(
                _ => _hubConnection.Reconnected += OnReconnected,
                _ => _hubConnection.Reconnected -= OnReconnected)
            .Subscribe();

        _hubConnection.On<string, string>("ReceiveMessage",
            MessageReceived);

    }

    private void MessageReceived(string user, string message)
    {
        _messages.OnNext(new ChatMessage(user, message));
        Console.WriteLine($"{user}: {message}");
    }


    private static Task OnReconnecting(Exception arg)
    {
        Console.WriteLine("Reconnecting");
        return Task.CompletedTask;
    }

    private static Task OnReconnected(string arg)
    {
        Console.WriteLine("Reconnected");
        return Task.CompletedTask;
    }

    private static Task OnConnectionClosed(Exception arg)
    {
        Console.WriteLine("Connection closed");
        return Task.CompletedTask;
    }

    private async Task CheckConnection()
    {
        if (_hubConnection is not { State: HubConnectionState.Connected })
        {
            await Connect();
        }
    }

    public async Task Disconnect()
    {
        try
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }
        finally
        {
            _hubConnection = null;
        }
    }

    public async Task<string> Send()
    {
        await CheckConnection();

        return await _hubConnection.InvokeAsync<string>("Send",
                "Hello World")
            .ContinueWith(p => p.IsFaulted ? "Some error occurred" : p.Result);
    }

}