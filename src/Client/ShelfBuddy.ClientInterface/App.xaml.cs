namespace ShelfBuddy.ClientInterface
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            UserAppTheme = AppTheme.Light; // Force light mode
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new MainPage()) { Title = "ShelfBuddy.ClientInterface" };
        }
    }
}
