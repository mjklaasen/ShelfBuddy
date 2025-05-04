using Microsoft.Extensions.Logging;

namespace ShelfBuddy.ClientInterface
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddMauiBlazorWebView();

            builder.Services.AddHttpClient("api", client =>
                {
                    
#if ANDROID
                    if (Environment.OSVersion.VersionString.Contains("35"))
                    {
                        // 10.0.2.2 is the special IP address to connect to the host machine from Android emulator
                        client.BaseAddress = new Uri("https://10.0.2.2:7088");
                    }
                    else
                    {
                        // For physical phone, but doesn't work, need to fix.
                        client.BaseAddress = new Uri("https://192.168.178.50:7088");
                    }
                    
#else
                    client.BaseAddress = new Uri("https://localhost:7088");
#endif
                })
#if ANDROID
                // Configure the message handler for Android to trust the development certificate
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = new HttpsClientHandlerService();
                    return handler.GetPlatformMessageHandler();
                })
#endif
                ;


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            builder.Services.AddSingleton(Preferences.Default);

            builder.Services.AddSingleton<IUserService>(sp =>
                new MockUserService(sp.GetRequiredService<IPreferences>()));
            return builder.Build();
        }
    }
}
