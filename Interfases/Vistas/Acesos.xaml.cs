using Interfases.VistaModel;
using Microsoft.Maui.Controls;

namespace Interfases.Vistas;

public partial class Acesos : ContentPage
{
	public Acesos()
	{
		InitializeComponent();
        BindingContext = new AcesoMV();

    }

    private void Button_Clicked(object sender, EventArgs e)
    {

    }

    private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {

    }

    private void Button_Clicked_1(object sender, EventArgs e)
    {

    }
}