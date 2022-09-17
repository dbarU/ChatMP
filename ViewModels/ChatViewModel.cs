using System.Collections.ObjectModel;
using ChatClient.Hubs;

namespace ChatClient.ViewModels;

public class ChatViewModel : BindableBase
{

    private string _userName;
    private string _chattingWith;
    private ObservableCollection<MessageViewModel> _messages;

    private readonly ChatBridge _signalRClientService;
    public ChatViewModel(ChatBridge signalRClientService)
    {
        _signalRClientService = signalRClientService;
        Messages = new ObservableCollection<MessageViewModel>();
        _signalRClientService.Messages.Subscribe(msg => Messages.Add(new MessageViewModel(msg)));
    }

    public DelegateCommand OnLoadedCommand => new(StartUp);
    public DelegateCommand OnUnloadedCommand => new(ShutDown);

    private async void ShutDown()
    {
        await _signalRClientService.Disconnect();
    }
    private async void StartUp()
    {
        await _signalRClientService.Connect();
    }

    public ObservableCollection<MessageViewModel> Messages
    {
        get => _messages;
        set { SetProperty(ref _messages, value); }
    }

    public string ChattingWith
    {
        get => _chattingWith;
        set { SetProperty(ref _chattingWith, value); }
    }

    public string UserName
    {
        get => _userName;
        set { SetProperty(ref _userName, value); }
    }
}
