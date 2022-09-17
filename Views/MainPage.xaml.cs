using ChatClient.Hubs;
using ChatClient.ViewModels;

namespace ChatClient;

public partial class MainPage : ContentPage
{

    public MainPage()
    {
        InitializeComponent();
        Loaded += MainPage_Loaded;
        Unloaded += MainPage_Unloaded;
    }

    private void MainPage_Unloaded(object sender, EventArgs e)
    {
        if (this.BindingContext is not ChatViewModel chatViewModel)
            return;

        chatViewModel.OnUnloadedCommand.Execute();
    }

    private void MainPage_Loaded(object sender, EventArgs e)
    {
        if (this.BindingContext is not ChatViewModel chatViewModel)
            return;

        chatViewModel.OnLoadedCommand.Execute();
    }
}
