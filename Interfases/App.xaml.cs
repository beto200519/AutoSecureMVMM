using Interfases.Vistas;

namespace Interfases
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new Login());
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            // Si estás en .NET MAUI, esta parte no se usa, ya que lo has configurado en el constructor de App()
            return base.CreateWindow(activationState);
        }
    }
}