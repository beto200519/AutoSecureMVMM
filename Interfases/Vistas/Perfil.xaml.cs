using Interfases.VistaModel;

namespace Interfases.Vistas;

public partial class Perfil : ContentPage
{
	public Perfil()
	{
		InitializeComponent();
        BindingContext = new PerfilMV();

    }
    protected async void VolLogClic(object sender, EventArgs e)
    {
      

        bool confirmacion = await DisplayAlert("Cerrar Sesi�n", "�Est� seguro que desea cerrar sesi�n?", "S�", "No");

        if (confirmacion)
        {
         
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}