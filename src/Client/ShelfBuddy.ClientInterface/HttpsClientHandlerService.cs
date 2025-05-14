#if ANDROID
using Xamarin.Android.Net;

namespace ShelfBuddy.ClientInterface;

public class HttpsClientHandlerService
{
    public static HttpMessageHandler GetPlatformMessageHandler()
    {
        var handler = new AndroidMessageHandler
        {
            ServerCertificateCustomValidationCallback = (_, _, _, _) => true
        };
        return handler;
    }

    public void SetupClientHandler()
    {
        var handler = new HttpClientHandler();
        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
    }
}
#endif