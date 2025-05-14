using Microsoft.Extensions.Logging;
using ShelfBuddy.ClientInterface.Services;

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
                    client.BaseAddress = Environment.OSVersion.VersionString.Contains("35")
                        ? new Uri(
                            "https://10.0.2.2:7088") // 10.0.2.2 is the special IP address to connect to the host machine from Android emulator
                        : new Uri("https://192.168.178.50:7088"); // For physical phone, but doesn't work, need to fix.
                    client.Timeout = TimeSpan.FromSeconds(10);
#else
                    client.BaseAddress = new Uri("https://localhost:7088");
#endif
                })
#if ANDROID
                // Configure the message handler for Android to trust the development certificate
                .ConfigurePrimaryHttpMessageHandler(() =>
                {
                    var handler = HttpsClientHandlerService.GetPlatformMessageHandler();
                    if (handler is HttpClientHandler httpHandler)
                    {
                        httpHandler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
                    }

                    return handler;
                })
#endif
                .AddHttpMessageHandler<HttpExceptionHandler>();


#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
    		builder.Logging.AddDebug();
#endif

            builder.Services
                .AddSingleton(Preferences.Default)
                .AddSingleton<IUserService>(sp => new MockUserService(sp.GetRequiredService<IPreferences>()))
                .AddScoped<IInventoryStateService, InventoryStateService>()
                .AddSingleton<ErrorHandlingService>()
                .AddTransient<HttpExceptionHandler>()
                .AddScoped<IInventoryService, InventoryService>()
                .AddScoped<IProductService, ProductService>();

            return builder.Build();
        }
    }

    public class HttpExceptionHandler(ErrorHandlingService errorService) : DelegatingHandler
    {
        private readonly ErrorHandlingService _errorService = errorService;

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            try
            {
                var response = await base.SendAsync(request, cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                // Log with more detailed information to help diagnose
                Console.WriteLine($"HTTP Handler Exception Type: {ex.GetType().FullName}");
                Console.WriteLine($"HTTP Handler Exception Message: {ex.Message}");
                Console.WriteLine($"HTTP Handler Stack Trace: {ex.StackTrace}");

                // Ensure we capture all types of exceptions, not just HttpRequestException
#if ANDROID
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    _errorService.ReportError(ex, $"Network error: {request.Method} {request.RequestUri}");
                });
#else
                _errorService.ReportError(ex, $"Network error: {request.Method} {request.RequestUri}");
#endif
                throw; // Still rethrow so caller can handle if needed
            }
        }
    }
}
