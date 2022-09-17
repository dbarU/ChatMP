using ChatClient.Hubs;
using ChatClient.ViewModels;
using ChatClient.Views;
using Microsoft.Maui.Hosting;

namespace ChatClient;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .UsePrism(prism =>
                    prism.ConfigureServices(services =>
                    {
                        services.RegisterGlobalNavigationObserver();
                        services.AddSingleton<ChatBridge>();
                        services.RegisterForNavigation<MessageView, MessageViewModel>();
                        services.RegisterForNavigation<MainPage, ChatViewModel>();
                    })
                    .AddGlobalNavigationObserver(context
                        => context.Subscribe(x =>
                            {
                                Console.WriteLine(x.Type == NavigationRequestType.Navigate
                                    ? $"Navigation: {x.Uri}"
                                    : $"Navigation: {x.Type}");

                                var status = x.Cancelled ? "Cancelled" : x.Result.Success ? "Success" : "Failed";

                                Console.WriteLine($"Result: {status}");

                                if (status != "Failed" || string.IsNullOrEmpty(x.Result?.Exception?.Message))
                                    return;

                                Console.Error.WriteLine(x.Result.Exception.Message);
                            })
                        )
                    .OnAppStart(navigationService =>
                    {
                        navigationService.CreateBuilder()
                            //.AddSegment("MainPage")
                            //.AddNavigationPage()
                            .AddSegment<ChatViewModel>()
                            //.AddSegment("MessageView")
                            .Navigate(HandleNavigationError);


                    })
                )
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        return builder.Build();
    }

    private static void HandleNavigationError(Exception ex)
    {
        Console.WriteLine(ex);
        System.Diagnostics.Debugger.Break();
    }
}
