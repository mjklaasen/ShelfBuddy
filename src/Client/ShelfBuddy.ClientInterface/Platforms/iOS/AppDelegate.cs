using Foundation;

namespace ShelfBuddy.ClientInterface
{
    [Register("AppDelegate")]
    internal sealed class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp();
        }
    }
}
