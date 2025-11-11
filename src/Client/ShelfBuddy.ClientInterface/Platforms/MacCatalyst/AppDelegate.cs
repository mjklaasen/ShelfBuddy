using Foundation;

namespace ShelfBuddy.ClientInterface
{
    [Register("AppDelegate")]
    internal class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp();
        }
    }
}
