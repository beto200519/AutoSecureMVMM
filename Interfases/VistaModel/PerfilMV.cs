using Interfases.VistaModel;
using System.ComponentModel;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

public class PerfilMV : INotifyPropertyChanged
{
    private ObservableCollection<PerfilModelo> _perfiles;
    private ObservableCollection<PerfilModelo> _perfilesFiltrados;
    private PerfilModelo _perfilActual;
    private string _busquedaTexto;
    private string _mensajeAccion;

    public ObservableCollection<PerfilModelo> Perfiles
    {
        get => _perfiles;
        set { _perfiles = value; OnPropertyChanged(); }
    }

    public ObservableCollection<PerfilModelo> PerfilesFiltrados
    {
        get => _perfilesFiltrados;
        set { _perfilesFiltrados = value; OnPropertyChanged(); }
    }

    public PerfilModelo PerfilActual
    {
        get => _perfilActual;
        set { _perfilActual = value ?? new PerfilModelo(); OnPropertyChanged(); }
    }

    public string BusquedaTexto
    {
        get => _busquedaTexto;
        set { _busquedaTexto = value; OnPropertyChanged(); }
    }

    public string MensajeAccion
    {
        get => _mensajeAccion;
        set { _mensajeAccion = value; OnPropertyChanged(); }
    }

    public ICommand BuscarCommand { get; }
    public ICommand FiltrarPerfilesCommand { get; }
    public ICommand EditarPerfilCommand { get; }
    public ICommand CrearPerfilCommand { get; }
    public ICommand GuardarPerfilCommand { get; }
    public ICommand EliminarPerfilCommand { get; }

    public PerfilMV()
    {
        _perfiles = new ObservableCollection<PerfilModelo>
            {
                new PerfilModelo { IdPerfil = 1, Nombre = "Norberto", Apellido = "Carrillo", TipoPerfil = "Administrador", Foto = "foto1.jpg", EstadoActivo = true },
                new PerfilModelo { IdPerfil = 2, Nombre = "Nicol", Apellido = "Gastelum", TipoPerfil = "Usuario", Foto = "foto2.jpg", EstadoActivo = true },
                new PerfilModelo { IdPerfil = 3, Nombre = "Jose Luis", Apellido = "Martínez", TipoPerfil = "Usuario", Foto = "foto3.jpg", EstadoActivo = false }
            };

        _perfilesFiltrados = new ObservableCollection<PerfilModelo>(_perfiles);
        _perfilActual = new PerfilModelo();

        BuscarCommand = new Command(BuscarPerfiles);
        FiltrarPerfilesCommand = new Command<string>(FiltrarPerfiles);
        EditarPerfilCommand = new Command<PerfilModelo>(EditarPerfil);
        CrearPerfilCommand = new Command(CrearPerfil);
        GuardarPerfilCommand = new Command(GuardarPerfil);
        EliminarPerfilCommand = new Command(EliminarPerfil);
    }

    private void EditarPerfil(PerfilModelo perfil)
    {
        if (perfil != null)
        {
            PerfilActual = perfil;
        }
    }

    private void BuscarPerfiles()
    {
        PerfilesFiltrados = string.IsNullOrWhiteSpace(BusquedaTexto)
            ? new ObservableCollection<PerfilModelo>(_perfiles)
            : new ObservableCollection<PerfilModelo>(_perfiles.Where(p => p.Nombre.ToLower().Contains(BusquedaTexto.ToLower())));
    }

    private void FiltrarPerfiles(string filtro)
    {
        PerfilesFiltrados = filtro == "Todos"
            ? new ObservableCollection<PerfilModelo>(_perfiles)
            : new ObservableCollection<PerfilModelo>(_perfiles.Where(p => p.TipoPerfil == filtro));
    }

    private void CrearPerfil()
    {
        PerfilActual = new PerfilModelo();
        MensajeAccion = "Nuevo perfil creado.";
    }

    private void GuardarPerfil()
    {
        if (!Perfiles.Contains(PerfilActual))
        {
            _perfiles.Add(PerfilActual);
        }
        MensajeAccion = "Perfil guardado correctamente.";
        PerfilesFiltrados = new ObservableCollection<PerfilModelo>(_perfiles);
    }

    private void EliminarPerfil()
    {
        if (Perfiles.Contains(PerfilActual))
        {
            _perfiles.Remove(PerfilActual);
            PerfilActual = new PerfilModelo();
            MensajeAccion = "Perfil eliminado correctamente.";
            PerfilesFiltrados = new ObservableCollection<PerfilModelo>(_perfiles);
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

public class Command : ICommand
{
    private readonly Action _execute;
    private readonly Func<bool> _canExecute;

    public event EventHandler CanExecuteChanged;

    public Command(Action execute, Func<bool> canExecute = null)
    {
        _execute = execute;
        _canExecute = canExecute;
    }

    public bool CanExecute(object parameter) => _canExecute == null || _canExecute();
    public void Execute(object parameter) => _execute();
}