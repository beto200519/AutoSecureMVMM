namespace Interfases.Vistas;

public partial class Registro : ContentPage
{
	public Registro()
	{
		InitializeComponent();
	}
    private async void VolverLoginClic(object sender, EventArgs e)
    {
        // Navegar a la página de Login
        await Navigation.PushAsync(new Login());
    }
   
}