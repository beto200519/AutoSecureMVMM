using Interfases.Modelo;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using Interfases.Vistas;

namespace Interfases.VistaModel;
public class AcesoMV : BindableObject
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private ObservableCollection<acesoModel> _usuarios;
    private ObservableCollection<acesoModel> _usuariosFiltrados;
    private acesoModel _usuarioActual;
    private string _mensajeAccion;
    private string _busquedaTexto;
    private string _rolSeleccionado;
    private readonly string apiUrl = "https://qmw9l8hh-5192.usw3.devtunnels.ms/api/Usuario";

    // Nuevas propiedades para horas descompuestas
    private string _horaInicio;
    private string _horaFin;

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

    public string RolSeleccionado
    {
        get => _rolSeleccionado;
        set { _rolSeleccionado = value; OnPropertyChanged(); FiltrarUsuarios(_rolSeleccionado); }
    }

    public string MensajeAccion
    {
        get => _mensajeAccion;
        set { _mensajeAccion = value; OnPropertyChanged(); }
    }

    public string HoraInicio
    {
        get => _horaInicio;
        set { _horaInicio = value; OnPropertyChanged(); }
    }

    public string HoraFin
    {
        get => _horaFin;
        set { _horaFin = value; OnPropertyChanged(); }
    }

    public ICommand CrearUsuarioCommand { get; }
    public ICommand GuardarUsuarioCommand { get; }
    public ICommand EliminarUsuarioCommand { get; }
    public ICommand EditarUsuarioCommand { get; }

    public AcesoMV()
    {
        _usuarios = new ObservableCollection<acesoModel>();
        _usuariosFiltrados = new ObservableCollection<acesoModel>();
        _usuarioActual = new acesoModel();

        CrearUsuarioCommand = new Command(async () => await CrearUsuario());
        GuardarUsuarioCommand = new Command(async () => await GuardarUsuario());
        EliminarUsuarioCommand = new Command(async () => await EliminarUsuario());
        EditarUsuarioCommand = new Command(() => EditarUsuario());

        _ = CargarUsuarios(); // Carga inicial de usuarios
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

                if (usuariosDB != null)
                {
                    Usuarios = usuariosDB;
                    UsuariosFiltrados = new ObservableCollection<acesoModel>(usuariosDB);
                    MensajeAccion = "Usuarios cargados correctamente.";
                }
                else
                {
                    MensajeAccion = "No se encontraron usuarios.";
                }
            }
            else
            {
                MensajeAccion = "Error al obtener usuarios.";
            }
        }
        catch (Exception ex)
        {
            MensajeAccion = $"Error de conexión: {ex.Message}";
        }
    }
    public void FiltrarUsuarios(string rol)
    {
        UsuariosFiltrados = string.IsNullOrWhiteSpace(rol)
            ? new ObservableCollection<acesoModel>(_usuarios)
            : new ObservableCollection<acesoModel>(_usuarios.Where(u => u.Rol?.Equals(rol, StringComparison.OrdinalIgnoreCase) ?? false));
    }

    public void BuscarUsuarios()
    {
        UsuariosFiltrados = string.IsNullOrWhiteSpace(BusquedaTexto)
            ? new ObservableCollection<acesoModel>(_usuarios)
            : new ObservableCollection<acesoModel>(_usuarios.Where(u => u.Nombre?.ToLower().Contains(BusquedaTexto.ToLower()) ?? false));
    }

    private void EditarUsuario()
    {
        var usuario = Usuarios.FirstOrDefault(u => u.Id == UsuarioActual.Id);
        if (usuario != null)
        {
            UsuarioActual = usuario;
            HoraInicio = usuario.HoraInicio;
            HoraFin = usuario.HoraFin;
            MensajeAccion = $"Usuario {UsuarioActual.Nombre} seleccionado para editar.";
        }
        else
        {
            MensajeAccion = "Usuario no encontrado.";
        }
    }

    private async Task CrearUsuario()
    {
        if (!EsValido())
            return;

        try
        {
            UsuarioActual.Horas = $"{HoraInicio}-{HoraFin}";
            var content = new StringContent(JsonConvert.SerializeObject(UsuarioActual), Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(apiUrl, content);

            if (response.IsSuccessStatusCode)
            {
                Usuarios.Add(UsuarioActual);
                MensajeAccion = "Usuario creado correctamente.";
                LimpiarCampos();
            }
            else
            {
                MensajeAccion = "Error al crear usuario.";
            }
        }
        catch (Exception ex)
        {
            MensajeAccion = $"Error: {ex.Message}";
        }
    }

    private async Task GuardarUsuario()
    {
        if (!EsValido())
            return;

        try
        {
            UsuarioActual.Horas = $"{HoraInicio}-{HoraFin}";
            var content = new StringContent(JsonConvert.SerializeObject(UsuarioActual), Encoding.UTF8, "application/json");
            var response = await _httpClient.PutAsync($"{apiUrl}/{UsuarioActual.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                var usuarioModificado = Usuarios.FirstOrDefault(u => u.Id == UsuarioActual.Id);
                if (usuarioModificado != null)
                {
                    Usuarios.Remove(usuarioModificado);
                    Usuarios.Add(UsuarioActual);
                }
                MensajeAccion = "Usuario actualizado correctamente.";
                LimpiarCampos();
            }
            else
            {
                MensajeAccion = "Error al guardar usuario.";
            }
        }
        catch (Exception ex)
        {
            MensajeAccion = $"Error: {ex.Message}";
        }
    }

    private async Task EliminarUsuario()
    {
        if (string.IsNullOrEmpty(UsuarioActual.Id))
        {
            MensajeAccion = "Seleccione un usuario para eliminar.";
            return;
        }

        try
        {
            var response = await _httpClient.DeleteAsync($"{apiUrl}/{UsuarioActual.Id}");

            if (response.IsSuccessStatusCode)
            {
                Usuarios.Remove(UsuarioActual);
                MensajeAccion = "Usuario eliminado correctamente.";
            }
            else
            {
                MensajeAccion = "Error al eliminar usuario.";
            }
        }
        catch (Exception ex)
        {
            MensajeAccion = $"Error: {ex.Message}";
        }
    }

    private bool EsValido()
    {
        if (string.IsNullOrWhiteSpace(UsuarioActual.Nombre) ||
            string.IsNullOrWhiteSpace(UsuarioActual.Correo) ||
            string.IsNullOrWhiteSpace(HoraInicio) ||
            string.IsNullOrWhiteSpace(HoraFin))
        {
            MensajeAccion = "Todos los campos son obligatorios.";
            return false;
        }

        return true;
    }

    private void LimpiarCampos()
    {
        UsuarioActual = new acesoModel();
        HoraInicio = string.Empty;
        HoraFin = string.Empty;
    }
}