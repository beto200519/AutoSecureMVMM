using Interfases.VistaModel;

namespace Interfases.Vistas;

public partial class Solicitud : ContentPage
{
	public Solicitud()
	{
		InitializeComponent();
        BindingContext = new SolicitudMV();
    }
}