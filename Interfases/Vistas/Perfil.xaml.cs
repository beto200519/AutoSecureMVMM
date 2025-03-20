using Interfases.VistaModel;

namespace Interfases.Vistas;

public partial class Perfil : ContentPage
{
	public Perfil()
	{
		InitializeComponent();
        BindingContext = new PerfilMV();

    }
}