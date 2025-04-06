using Interfases.Servicios;
using Interfases.VistaModel;
using Microsoft.Extensions.Logging;

namespace Interfases;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
			});
        builder.Services.AddSingleton<PerfilMV>();
        builder.Services.AddTransient<LoginVM>();
        builder.Services.AddSingleton<AuthService>();
        builder.Services.AddTransient<Interfases.Vistas.Login>();



#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}
