using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Interfases.Modelo;

namespace Interfases.VistaModel;

public class AcesoMV : INotifyPropertyChanged
{
    private ObservableCollection<acesoModel> _usuarios;
    private ObservableCollection<acesoModel> _usuariosFiltrados;
    private acesoModel _usuarioActual;
    private string _busquedaTexto;
    private string _mensajeAccion;

    public ObservableCollection<acesoModel> Usuarios { get => _usuarios; set { _usuarios = value; OnPropertyChanged(); } }
    public ObservableCollection<acesoModel> UsuariosFiltrados { get => _usuariosFiltrados; set { _usuariosFiltrados = value; OnPropertyChanged(); } }
    public acesoModel UsuarioActual { get => _usuarioActual; set { _usuarioActual = value ?? new acesoModel(); OnPropertyChanged(); } }
    public string BusquedaTexto { get => _busquedaTexto; set { _busquedaTexto = value; OnPropertyChanged(); } }
    public string MensajeAccion { get => _mensajeAccion; set { _mensajeAccion = value; OnPropertyChanged(); } }

    public ICommand BuscarCommand { get; }
    public ICommand FiltrarUsuariosCommand { get; }
    public ICommand EditarUsuarioCommand { get; }
    public ICommand CrearUsuarioCommand { get; }
    public ICommand GuardarUsuarioCommand { get; }
    public ICommand EliminarUsuarioCommand { get; }

    public AcesoMV()
    {
        _usuarios = new ObservableCollection<acesoModel>
            {
                new acesoModel { Id = 1, Nombre = "Norberto Carrillo", Correo = "Norberto@Gmail.com", Password = "1234", Rol = "Administrador", HoraPermitida = new TimeSpan(8, 0, 0), DiaPermitido = DateTime.Today, PuertaPermitida = "Principal", CuentaHabilitada = true },
                new acesoModel { Id = 2, Nombre = "Nicol gastelum", Correo = "Nicol@Gmail.com", Password = "1234", Rol = "Usuario", HoraPermitida = new TimeSpan(9, 0, 0), DiaPermitido = DateTime.Today, PuertaPermitida = "Secundaria", CuentaHabilitada = true },
                new acesoModel { Id = 3, Nombre = "Jose Luis", Correo = "Jose@gmail.com", Password = "1234", Rol = "Usuario", HoraPermitida = new TimeSpan(10, 0, 0), DiaPermitido = DateTime.Today, PuertaPermitida = "Principal", CuentaHabilitada = false }
            };

        _usuariosFiltrados = new ObservableCollection<acesoModel>(_usuarios);
        _usuarioActual = new acesoModel();

        BuscarCommand = new Command(BuscarUsuarios);
        FiltrarUsuariosCommand = new Command<string>(FiltrarUsuarios);
        EditarUsuarioCommand = new Command<acesoModel>(EditarUsuario);
        CrearUsuarioCommand = new Command(CrearUsuario);
        GuardarUsuarioCommand = new Command(GuardarUsuario);
        EliminarUsuarioCommand = new Command(EliminarUsuario);
    }

    private void EditarUsuario(acesoModel usuario)
    {
        if (usuario != null)
        {
            UsuarioActual = usuario; // Asignar directamente el usuario
        }
    }

    private void BuscarUsuarios()
    {
        UsuariosFiltrados = string.IsNullOrWhiteSpace(BusquedaTexto)
            ? new ObservableCollection<acesoModel>(_usuarios)
            : new ObservableCollection<acesoModel>(_usuarios.Where(u => u.Nombre.ToLower().Contains(BusquedaTexto.ToLower())));
    }

    private void FiltrarUsuarios(string filtro)
    {
        UsuariosFiltrados = filtro == "Todos"
            ? new ObservableCollection<acesoModel>(_usuarios)
            : new ObservableCollection<acesoModel>(_usuarios.Where(u => u.Rol == filtro));
    }

    private void CrearUsuario() { MensajeAccion = "Usuario creado correctamente."; }
    private void GuardarUsuario() { MensajeAccion = "Cambios guardados correctamente."; }
    private void EliminarUsuario() { MensajeAccion = "Usuario eliminado correctamente."; }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}