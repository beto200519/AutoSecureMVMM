using Interfases.Servicios;
using Interfases.VistaModel;

namespace Interfases.Vistas
{
    public partial class Login : ContentPage
    {
        // Constructor sin par�metros para el preview o inicializaci�n simple
        public Login()
        {
            InitializeComponent();
            BindingContext = new LoginVM(new AuthService(), Navigation);  // Asignar LoginVM directamente si no se pasa ViewModel
        }
    }
}