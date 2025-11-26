#if ANDROID
using Xamarin.Android.Net;

namespace ShelfBuddy.ClientInterface;

public static class HttpsClientHandlerService
{
    public static HttpMessageHandler PlatformMessageHandler => new AndroidMessageHandler
    {
        ServerCertificateCustomValidationCallback = (_, _, _, _) => true
    };
}
#endif
