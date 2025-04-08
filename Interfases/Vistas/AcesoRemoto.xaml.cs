using Interfases.VistaModel;

namespace Interfases.Vistas;

public partial class AcesoRemoto : ContentPage
{
	public AcesoRemoto()
	{
		InitializeComponent();
        BindingContext = new AccesoRemotoVM(); // Asegura que la ViewModel está enlazada
    }

    private async void OnBackButtonClicked(object sender, EventArgs e)
    {
        var button = (ImageButton)sender;

        // Animación: Desplazamiento de izquierda a derecha.
        var moveAnimation = new Animation(v =>
        {
            button.TranslationX = v;
        }, 0, 100); // Desplaza el botón 100 unidades hacia la derecha

        // Ejecutar la animación en el botón.
        moveAnimation.Commit(button, "MoveButton", 16, 500, Easing.CubicOut);

        // Aquí puedes agregar la lógica para la navegación o cualquier otro comportamiento.
        await Navigation.PopAsync(); // Vuelve a la página anterior
    }
}