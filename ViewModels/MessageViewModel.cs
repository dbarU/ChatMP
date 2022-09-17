using ChatClient.Hubs;
using static System.String;

namespace ChatClient.ViewModels;
public class MessageViewModel : BindableBase
{
    private string _message = Empty;

    public MessageViewModel(ChatMessage chatMessage)
    {
        Message = chatMessage.Message;
    }
    public string Message
    {
        get => _message;
        set => SetProperty(ref _message, value);
    }
}
