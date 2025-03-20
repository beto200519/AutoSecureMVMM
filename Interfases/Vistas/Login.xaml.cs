namespace Interfases.Vistas;

public partial class Login : ContentPage
{
    public Login()
    {
        InitializeComponent();
    }
    private async void EntrarClic(object sender, EventArgs e)
    {
        // Navegar a la p�gina de Login
        await Navigation.PushAsync(new AppShell());
    }
    private async void Registrarclic(object sender, EventArgs e)
    {
        // Navegar a la p�gina de Registro
        await Navigation.PushAsync(new Registro());
    }
     private async void OnContactUsClicked()
        {
            // Direcci�n de correo a la que se enviar� el email
            string email = "norbertocarrillo2005@gmail.com";
            string subject = "Consulta sobre AutoSecure";  // Puedes cambiar el asunto
            string body = "Hola, me gustar�a obtener m�s informaci�n sobre..."; // Cuerpo del mensaje

            // Usamos el Launcher para abrir el cliente de correo
            var emailUri = new Uri($"mailto:{email}?subject={subject}&body={body}");
            await Launcher.OpenAsync(emailUri);
        }
}