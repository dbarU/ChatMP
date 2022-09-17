using Microsoft.AspNetCore.SignalR.Client;

namespace ChatClient.Hubs;

public class ReactiveChatBridge : IReactiveChatBridge
{
    private readonly HubConnection _connection;
    private Task _onNextTask;

    public ReactiveChatBridge()
    {
        _connection = new HubConnectionBuilder()

            .WithAutomaticReconnect()
            // .WithUrl("https://localhost:5001/chatHub")
            .Build();

        _connection.On<string, string>("MessageReceived",
            (user, message) =>
        {
            Console.WriteLine($"{user}: {message}");
        });

        Task.Run(async () => await Dispatcher
            .GetForCurrentThread()!
            .DispatchAsync(() => _connection.StartAsync()));

    }

    /// <inheritdoc />
    public async Task SendMessage(IMessage message)
    {
        await _connection.InvokeCoreAsync("SendMessage", args: new object[] { message });
    }

    /// <inheritdoc />
    public IDisposable Subscribe(IObserver<IMessage> observer)
    {
        return _connection.On<IMessage>("MessageReceived", observer.OnNext);
    }

    /// <inheritdoc />
    public void OnCompleted()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void OnError(Exception error)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void OnNext(IMessage value)
    {
        Task.Run(async () => await _connection
             .InvokeCoreAsync("SendMessage",
                 args: new object[] { value }));

    }
}

public record Message(string Author, string Text, string Destination) : IMessage;


public interface IReactiveChatBridge : IChatBridge, IObservable<IMessage>, IObserver<IMessage>
{
}
public interface IChatBridge
{
    Task SendMessage(IMessage message);
}

public interface IMessage
{
    string Author { get; init; }
    string Text { get; init; }
    string Destination { get; init; }

}
