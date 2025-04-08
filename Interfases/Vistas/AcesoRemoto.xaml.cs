using Interfases.VistaModel;

namespace Interfases.Vistas;

public partial class AcesoRemoto : ContentPage
{
	public AcesoRemoto()
	{
		InitializeComponent();
        BindingContext = new AccesoRemotoVM(); // Asegura que la ViewModel est� enlazada
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        var button = (ImageButton)sender;

        // Animaci�n: Desplazamiento de izquierda a derecha.
        var moveAnimation = new Animation(v =>
        {
            button.TranslationX = v;
        }, 0, 100); // Desplaza el bot�n 100 unidades hacia la derecha

        // Ejecutar la animaci�n en el bot�n.
        moveAnimation.Commit(button, "MoveButton", 16, 500, Easing.CubicOut);

        // Aqu� puedes agregar la l�gica para la navegaci�n o cualquier otro comportamiento.
        await Navigation.PopAsync(); // Vuelve a la p�gina anterior
    }
}