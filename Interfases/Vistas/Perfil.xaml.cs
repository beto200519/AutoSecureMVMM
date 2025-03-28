using Interfases.VistaModel;

namespace Interfases.Vistas;

public partial class Perfil : ContentPage
{
	public Perfil()
	{
		InitializeComponent();
        BindingContext = new PerfilMV();

    }
    private async void VolLogClic(object sender, EventArgs e)
    {
        // Navegar a la página de Registro
        await Navigation.PushAsync(new Login());
    }
}