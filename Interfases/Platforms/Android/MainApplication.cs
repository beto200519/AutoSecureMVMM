using Android.App;
using Android.Runtime;
using Android.OS;
using System.Net;

namespace Interfases
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership) { }

        public override void OnCreate()
        {
            base.OnCreate();

            //  Solo para desarrollo - permite certificados no válidos
            ServicePointManager.ServerCertificateValidationCallback += (sender, cert, chain, sslPolicyErrors) => true;
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}
