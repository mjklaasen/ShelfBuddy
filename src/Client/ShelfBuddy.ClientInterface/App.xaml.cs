using ShelfBuddy.ClientInterface.Services;
using Microsoft.Maui.ApplicationModel;

namespace ShelfBuddy.ClientInterface
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
            TaskScheduler.UnobservedTaskException += OnUnobservedTaskException;
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "ShelfBuddy.ClientInterface" };
        }

        private static void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var exception = e.ExceptionObject as Exception ?? new Exception("Unknown error");
            Console.WriteLine($"Unhandled Exception: {exception.Message}");
            Console.WriteLine($"Exception Type: {exception.GetType().FullName}");

            // Use MainThread for Android
            MainThread.BeginInvokeOnMainThread(() => {
                var errorService = IPlatformApplication.Current?.Services.GetService<ErrorHandlingService>();
                errorService?.ReportError(exception, "Unhandled Application Exception");
            });
        }

        private static void OnUnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
        {
            Console.WriteLine($"Unobserved Task Exception: {e.Exception.Message}");

            // Use MainThread for Android
            MainThread.BeginInvokeOnMainThread(() => {
                var errorService = IPlatformApplication.Current?.Services.GetService<ErrorHandlingService>();
                errorService?.ReportError(e.Exception, "Unhandled Task Exception");
            });

            e.SetObserved(); // Prevent the app from crashing
        }
    }
}