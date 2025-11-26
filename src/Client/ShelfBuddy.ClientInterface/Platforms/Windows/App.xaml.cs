using Microsoft.UI.Xaml;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace ShelfBuddy.ClientInterface.WinUI
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
#pragma warning disable CA1515
    public partial class App : MauiWinUIApplication
#pragma warning restore CA1515
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            InitializeComponent();
        }

        protected override MauiApp CreateMauiApp()
        {
            return MauiProgram.CreateMauiApp();
        }
    }

}
