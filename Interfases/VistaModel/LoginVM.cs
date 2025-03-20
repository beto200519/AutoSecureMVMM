using System.Windows.Input;

namespace Interfases.VistaModel
{
    public class LoginVM : BindableObject
    {
        private string _email;
        private string _password;
       // private readonly MongoDBService _mongoDBService;

        public string Email
        {
            get => _email;
            set
            {
                _email = value;
                OnPropertyChanged();
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                _password = value;
                OnPropertyChanged();
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand ContactUsCommand { get; }

        public LoginVM()
        {
            /*      _mongoDBService = new MongoDBService();
                    LoginCommand = new Command(OnLogin);
                    ContactUsCommand = new Command(OnContactUsClicked);
                }

                private async void OnLogin()
                {
                 //   var user = await _mongoDBService.AuthenticateUser(Email, Password);
                    if (user != null)
                    {
                        await Application.Current.MainPage.DisplayAlert("Éxito", "Inicio de sesión exitoso", "OK");
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Credenciales incorrectas", "OK");
                    }
                */
        }
        private async void OnContactUsClicked()
        {
            // Dirección de correo a la que se enviará el email
            string email = "norbertocarrillo2005@gmail.com";
            string subject = "Consulta sobre AutoSecure";  // Puedes cambiar el asunto
            string body = "Hola, me gustaría obtener más información sobre..."; // Cuerpo del mensaje

            // Usamos el Launcher para abrir el cliente de correo
            var emailUri = new Uri($"mailto:{email}?subject={subject}&body={body}");
            await Launcher.OpenAsync(emailUri);
        }

    }
}