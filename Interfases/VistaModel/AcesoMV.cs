using Interfases.Modelo;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

using System.Windows.Input;

namespace Interfases.VistaModel
{
    public class AcesoMV : INotifyPropertyChanged
    {
        private readonly HttpClient _httpClient;
        private ObservableCollection<acesoModel> _usuarios;
        private ObservableCollection<acesoModel> _usuariosFiltrados;
        private acesoModel _usuarioActual;
        private string _busquedaTexto;
        private string _mensajeAccion;
        private readonly string apiUrl = "https://tu-api.com/api/usuarios"; // Asegúrate de cambiar esto por la URL correcta.

        public ObservableCollection<acesoModel> Usuarios
        {
            get => _usuarios;
            set { _usuarios = value; OnPropertyChanged(); }
        }

        public ObservableCollection<acesoModel> UsuariosFiltrados
        {
            get => _usuariosFiltrados;
            set { _usuariosFiltrados = value; OnPropertyChanged(); }
        }

        public acesoModel UsuarioActual
        {
            get => _usuarioActual;
            set { _usuarioActual = value ?? new acesoModel(); OnPropertyChanged(); }
        }

        public string BusquedaTexto
        {
            get => _busquedaTexto;
            set { _busquedaTexto = value; OnPropertyChanged(); BuscarUsuarios(); }
        }

        public string MensajeAccion
        {
            get => _mensajeAccion;
            set { _mensajeAccion = value; OnPropertyChanged(); }
        }

        public ICommand CrearUsuarioCommand { get; }
        public ICommand GuardarUsuarioCommand { get; }
        public ICommand EliminarUsuarioCommand { get; }

        public AcesoMV()
        {
            _httpClient = new HttpClient();
            _usuarios = new ObservableCollection<acesoModel>();
            _usuariosFiltrados = new ObservableCollection<acesoModel>();
            _usuarioActual = new acesoModel();

            CrearUsuarioCommand = new Command(async () => await CrearUsuario());
            GuardarUsuarioCommand = new Command(async () => await GuardarUsuario());
            EliminarUsuarioCommand = new Command(async () => await EliminarUsuario());

            _ = CargarUsuarios();
        }

        private async Task CargarUsuarios()
        {
            try
            {
                var response = await _httpClient.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var usuariosDB = JsonConvert.DeserializeObject<ObservableCollection<acesoModel>>(json);
                    Usuarios = usuariosDB;
                    UsuariosFiltrados = new ObservableCollection<acesoModel>(usuariosDB);
                }
                else
                {
                    MensajeAccion = "Error al obtener usuarios.";
                }
            }
            catch (Exception ex)
            {
                MensajeAccion = "Error de conexión: " + ex.Message;
            }
        }

        private void BuscarUsuarios()
        {
            UsuariosFiltrados = string.IsNullOrWhiteSpace(BusquedaTexto)
                ? new ObservableCollection<acesoModel>(_usuarios)
                : new ObservableCollection<acesoModel>(_usuarios.Where(u => u.Nombre.ToLower().Contains(BusquedaTexto.ToLower())));
        }

        private async Task CrearUsuario()
        {
            if (string.IsNullOrWhiteSpace(UsuarioActual.Nombre) ||
                string.IsNullOrWhiteSpace(UsuarioActual.Correo) ||
                string.IsNullOrWhiteSpace(UsuarioActual.Password) ||
                string.IsNullOrWhiteSpace(UsuarioActual.Rol))
            {
                MensajeAccion = "Todos los campos son obligatorios.";
                return;
            }

            var json = JsonConvert.SerializeObject(UsuarioActual);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Usuarios.Add(UsuarioActual);
                UsuariosFiltrados.Add(UsuarioActual);
                MensajeAccion = "Usuario creado correctamente.";
                UsuarioActual = new acesoModel(); // Reseteamos el formulario
            }
            else
            {
                MensajeAccion = "Error al crear usuario.";
            }
        }

        private async Task GuardarUsuario()
        {
            if (UsuarioActual == null || UsuarioActual.Id == 0) return;

            var json = JsonConvert.SerializeObject(UsuarioActual);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{apiUrl}/{UsuarioActual.Id}", content);

            MensajeAccion = response.IsSuccessStatusCode ? "Cambios guardados correctamente." : "Error al guardar cambios.";
        }

        private async Task EliminarUsuario()
        {
            if (UsuarioActual == null || UsuarioActual.Id == 0) return;

            var response = await _httpClient.DeleteAsync($"{apiUrl}/{UsuarioActual.Id}");

            if (response.IsSuccessStatusCode)
            {
                Usuarios.Remove(UsuarioActual);
                UsuariosFiltrados.Remove(UsuarioActual);
                MensajeAccion = "Usuario eliminado correctamente.";
                UsuarioActual = new acesoModel();
            }
            else
            {
                MensajeAccion = "Error al eliminar usuario.";
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
