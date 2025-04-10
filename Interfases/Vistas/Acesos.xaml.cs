using Interfases.VistaModel;

namespace Interfases.Vistas;

public partial class Acesos : ContentPage
{
	public Acesos()
	{
		InitializeComponent();
        BindingContext = new AcesoMV();

    }
}