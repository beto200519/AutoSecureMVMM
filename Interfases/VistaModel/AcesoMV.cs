using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Interfases.Modelos;
using Interfases.Servicios;
using Newtonsoft.Json;
using Microsoft.Maui.Controls;
using Interfases.Modelo;
using Interfases.Services;

namespace Interfases.VistaModel
{
    public class AcesoMV : BindableObject
    {
        private readonly HttpClientService _httpClientService;
        private readonly ApiService _apiService;
        private ObservableCollection<acesoModel> _usuarios;
        private ObservableCollection<acesoModel> _usuariosFiltrados;
        private acesoModel _usuarioActual;
        private string _mensajeAccion;
        private string _busquedaTexto;
        private string _rolSeleccionado;
        private bool _isBusy;
        private const string BaseApiUrl = "https://ml1ctcld-7149.usw3.devtunnels.ms/api/Usuario";

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
            set { _busquedaTexto = value; OnPropertyChanged(); AplicarFiltros(); }
        }

        public string RolSeleccionado
        {
            get => _rolSeleccionado;
            set { _rolSeleccionado = value; OnPropertyChanged(); AplicarFiltros(); }
        }

        public string MensajeAccion
        {
            get => _mensajeAccion;
            set { _mensajeAccion = value; OnPropertyChanged(); }
        }

        public bool IsBusy
        {
            get => _isBusy;
            set { _isBusy = value; OnPropertyChanged(); }
        }

        public ICommand CrearUsuarioCommand { get; }
        public ICommand GuardarUsuarioCommand { get; }
        public ICommand EliminarUsuarioCommand { get; }
        public ICommand EditarUsuarioCommand { get; }
        public ICommand FiltroCommand { get; }

        public AcesoMV()
        {
            _httpClientService = new HttpClientService();
            _apiService = new ApiService();
            _usuarios = new ObservableCollection<acesoModel>();
            _usuariosFiltrados = new ObservableCollection<acesoModel>();
            _usuarioActual = new acesoModel();

            // Inicializar comandos
            CrearUsuarioCommand = new Command(async () => await CrearUsuario());
            GuardarUsuarioCommand = new Command(async () => await GuardarUsuario());
            EliminarUsuarioCommand = new Command(async () => await EliminarUsuario());
            EditarUsuarioCommand = new Command<acesoModel>(EditarUsuario);
            FiltroCommand = new Command<string>(rol => RolSeleccionado = rol);

            // Cargar datos iniciales
            _ = CargarUsuariosIniciales();
        }

        private async Task CargarUsuariosIniciales()
        {
            try
            {
                IsBusy = true;
                await VerificarAutenticacion();
                await CargarUsuarios();
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task VerificarAutenticacion()
        {
            var token = await AuthService.GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                await Shell.Current.GoToAsync("//Login");
                throw new Exception("No autenticado");
            }
        }

        private async Task CargarUsuarios()
        {
            try
            {
                var client = await _httpClientService.GetAuthenticatedClientAsync();
                var response = await client.GetAsync($"{BaseApiUrl}/Get");

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var usuariosDB = JsonConvert.DeserializeObject<ObservableCollection<acesoModel>>(json);

                    Usuarios = new ObservableCollection<acesoModel>(usuariosDB);
                    AplicarFiltros();
                    MensajeAccion = "Usuarios cargados correctamente";
                }
                else
                {
                    MensajeAccion = "Error al obtener usuarios: " + response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                MensajeAccion = $"Error: {ex.Message}";
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
            }
        }

        private void AplicarFiltros()
        {
            var filtered = _usuarios.AsEnumerable();

            if (!string.IsNullOrEmpty(RolSeleccionado))
            {
                filtered = filtered.Where(u => u.Rol == RolSeleccionado);
            }

            if (!string.IsNullOrEmpty(BusquedaTexto))
            {
                filtered = filtered.Where(u =>
                    u.Nombre.Contains(BusquedaTexto, StringComparison.OrdinalIgnoreCase));
            }

            UsuariosFiltrados = new ObservableCollection<acesoModel>(filtered);
        }

        private void EditarUsuario(acesoModel usuario)
        {
            if (usuario != null)
            {
                UsuarioActual = new acesoModel
                {
                    Id = usuario.Id,
                    Nombre = usuario.Nombre,
                    Correo = usuario.Correo,
                    Clave = usuario.Clave,
                    Pin = usuario.Pin,
                    Rol = usuario.Rol
                };
                MensajeAccion = $"Editando: {usuario.Nombre}";
            }
        }

        private async Task CrearUsuario()
        {
            if (!EsValido()) return;

            try
            {
                IsBusy = true;
                var client = await _httpClientService.GetAuthenticatedClientAsync();
                var content = new StringContent(JsonConvert.SerializeObject(UsuarioActual),
                    Encoding.UTF8, "application/json");

                var response = await client.PostAsync($"{BaseApiUrl}/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    var nuevoUsuario = JsonConvert.DeserializeObject<acesoModel>(
                        await response.Content.ReadAsStringAsync());

                    Usuarios.Add(nuevoUsuario);
                    AplicarFiltros();
                    LimpiarCampos();
                    MensajeAccion = "¡Usuario creado!";
                }
                else
                {
                    MensajeAccion = "Error: " + await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                MensajeAccion = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task GuardarUsuario()
        {
            if (!EsValido()) return;

            try
            {
                IsBusy = true;
                var client = await _httpClientService.GetAuthenticatedClientAsync();
                var content = new StringContent(JsonConvert.SerializeObject(UsuarioActual),
                    Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"{BaseApiUrl}/Update/{UsuarioActual.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    var index = Usuarios.IndexOf(Usuarios.First(u => u.Id == UsuarioActual.Id));
                    Usuarios[index] = UsuarioActual;
                    AplicarFiltros();
                    LimpiarCampos();
                    MensajeAccion = "¡Cambios guardados!";
                }
                else
                {
                    MensajeAccion = "Error: " + await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                MensajeAccion = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task EliminarUsuario()
        {
            if (string.IsNullOrEmpty(UsuarioActual.Id))
            {
                MensajeAccion = "Seleccione un usuario";
                return;
            }

            bool confirmar = await Shell.Current.DisplayAlert(
                "Confirmar",
                $"¿Eliminar a {UsuarioActual.Nombre}?",
                "Sí", "No");

            if (!confirmar) return;

            try
            {
                IsBusy = true;
                var client = await _httpClientService.GetAuthenticatedClientAsync();
                var response = await client.DeleteAsync($"{BaseApiUrl}/Delete/{UsuarioActual.Id}");

                if (response.IsSuccessStatusCode)
                {
                    Usuarios.Remove(UsuarioActual);
                    AplicarFiltros();
                    LimpiarCampos();
                    MensajeAccion = "Usuario eliminado";
                }
                else
                {
                    MensajeAccion = "Error: " + await response.Content.ReadAsStringAsync();
                }
            }
            catch (Exception ex)
            {
                MensajeAccion = $"Error: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool EsValido()
        {
            if (string.IsNullOrWhiteSpace(UsuarioActual.Nombre))
            {
                MensajeAccion = "Nombre requerido";
                return false;
            }

            if (string.IsNullOrWhiteSpace(UsuarioActual.Correo))
            {
                MensajeAccion = "Correo requerido";
                return false;
            }

            if (UsuarioActual.Pin <= 0)
            {
                MensajeAccion = "PIN inválido";
                return false;
            }

            return true;
        }

        private void LimpiarCampos()
        {
            UsuarioActual = new acesoModel();
            MensajeAccion = string.Empty;
        }
    }
}