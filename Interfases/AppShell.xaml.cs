namespace Interfases
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            base.OnAppearing();
            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
            Shell.SetNavBarIsVisible(this, false); // Esto asegura que la barra de navegación no sea visible

        }
    }
}
