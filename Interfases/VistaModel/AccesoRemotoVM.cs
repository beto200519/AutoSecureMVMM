using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Interfases.Modelo;
using Interfases.Modelos;
using Interfases.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Interfases.VistaModel
{
    public partial class AccesoRemotoVM : ObservableObject
    {
        private readonly HttpClientService _httpClientService;
        private readonly string _apiAccesosUrl = "https://ml1ctcld-7149.usw3.devtunnels.ms/api/Accesos";
        private readonly string _apiUsuariosUrl = "https://ml1ctcld-7149.usw3.devtunnels.ms/api/Usuario";
        private readonly string _apiCircuitoUrl = "https://ml1ctcld-7149.usw3.devtunnels.ms/api/Circuito/Put/67ed8c3d1f95e236d4cff150";


        private readonly IMongoCollection<BsonDocument> _circuitoCollection;

        [ObservableProperty]
        private bool _puertaAbierta;

        [ObservableProperty]
        private bool _seguroActivado;

        // Propiedades derivadas para la UI
        public bool PuertaCerrada => !PuertaAbierta;
        public string EstadoPuerta => PuertaAbierta ? "Puerta Abierta" : "Puerta Cerrada";
        public string EstadoSeguro => SeguroActivado ? "Seguro Activado" : "Seguro Desactivado";
        public bool SeguroDesactivado => !SeguroActivado;

        public AccesoRemotoVM()
        {
            // Conexión a MongoDB
            var connectionString = "mongodb+srv://dye:tero@control.nmu0r.mongodb.net/?retryWrites=true&w=majority&appName=Control";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("Sistema");
            _circuitoCollection = database.GetCollection<BsonDocument>("Circuitos");

            // Inicialización de estados
            PuertaAbierta = false;
            SeguroActivado = true;
        }

        [RelayCommand]
        private async Task OnPuertaTapped()
        {
            if (SeguroActivado)
            {
                await Application.Current.MainPage.DisplayAlert("Acceso Denegado", "La puerta tiene el seguro activado.", "OK");
                return;
            }

            try
            {
                var filtro = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse("67ed8c3d1f95e236d4cff150"));
                var nuevoEstado = !PuertaAbierta; // Alternar el estado de la puerta
                var actualizacion = Builders<BsonDocument>.Update
                    .Set("Fecha", DateTime.UtcNow)
                    .Set("Estado", nuevoEstado);

                var resultado = await _circuitoCollection.UpdateOneAsync(filtro, actualizacion);

                if (resultado.ModifiedCount > 0)
                {
                    PuertaAbierta = nuevoEstado; // Actualizar el estado en la UI solo si la DB se actualizó
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "No se actualizó el estado de la puerta en la base de datos.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al actualizar la puerta: {ex.Message}", "OK");
            }
        }

        [RelayCommand]
        private async Task OnSeguroTapped()
        {
            try
            {
                var filtro = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse("67ed8c3d1f95e236d4cff150"));
                var nuevoEstadoSeguro = !SeguroActivado; // Alternar el estado del seguro
                var actualizacion = Builders<BsonDocument>.Update
                    .Set("Fecha", DateTime.UtcNow)
                    .Set("Estado", nuevoEstadoSeguro);

                var resultado = await _circuitoCollection.UpdateOneAsync(filtro, actualizacion);

                if (resultado.ModifiedCount > 0)
                {
                    SeguroActivado = nuevoEstadoSeguro; // Actualizar el estado en la UI solo si la DB se actualizó
                }
                else
                {
                    await Application.Current.MainPage.DisplayAlert("Advertencia", "No se actualizó el estado del seguro en la base de datos.", "OK");
                }
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", $"Error al actualizar el seguro: {ex.Message}", "OK");
            }
        }
    }
}
