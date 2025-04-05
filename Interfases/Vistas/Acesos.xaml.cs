using Interfases.Modelo;
using Interfases.VistaModel;

namespace Interfases.Vistas
{
    public partial class Acesos : ContentPage
    {
        private AcesoMV ViewModel;

        public Acesos()
        {
            InitializeComponent();
            ViewModel = new AcesoMV();
            BindingContext = ViewModel;
        }

        // Filtra todos los usuarios (sin ningún filtro de rol)
        private void FiltraTodos_Clicked(object sender, EventArgs e)
        {
            ViewModel.FiltrarUsuarios(""); // Muestra todos los usuarios
        }

        // Filtra usuarios con rol 'Administrador'
        private void FiltraAdmin_Clicked(object sender, EventArgs e)
        {
            ViewModel.FiltrarUsuarios("Administrador");
        }

        // Filtra usuarios con rol 'Usuario'
        private void FiltraUsuario_Clicked(object sender, EventArgs e)
        {
            ViewModel.FiltrarUsuarios("Usuario");
        }

        // Realiza la búsqueda de usuarios
        private void BuscarUsuario_Clicked(object sender, EventArgs e)
        {
            ViewModel.BuscarUsuarios();
        }

        // Actualiza el usuario actual cuando se selecciona un elemento en el CollectionView
        private void CollectionView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.FirstOrDefault() is acesoModel usuario)
            {
                ViewModel.UsuarioActual = usuario;
            }
        }
    }
}
