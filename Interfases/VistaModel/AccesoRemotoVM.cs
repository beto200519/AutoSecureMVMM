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
using System.Windows.Input;

namespace Interfases.VistaModel
{
    public partial class AccesoRemotoVM : ObservableObject
    {
        private readonly IMongoCollection<BsonDocument> _circuitoCollection;

        private bool _puertaAbierta;
        private int _tiempoRestante;
        public bool PuertaAbierta => _puertaAbierta;
        public bool PuertaCerrada => !_puertaAbierta;
        public string EstadoPuerta => _puertaAbierta ? "Puerta Abierta" : "Puerta Cerrada";
        public bool MostrarContador => _tiempoRestante > 0;
        public string Contador => $"{_tiempoRestante}s";

        [ObservableProperty]
        private string _estadoSeguro = "Seguro: Inactivo";

        [ObservableProperty]
        private bool _seguroActivado = false;

        [ObservableProperty]
        private bool _seguroDesactivado = true;

        public ICommand SeguroTappedCommand { get; }
        public ICommand DSeguroTappedCommand { get; }

        public AccesoRemotoVM()
        {
            var connectionString = "mongodb+srv://dye:tero@control.nmu0r.mongodb.net/?retryWrites=true&w=majority&appName=Control";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("Sistema");
            _circuitoCollection = database.GetCollection<BsonDocument>("Circuitos");

            SeguroTappedCommand = new AsyncRelayCommand(ActivarSeguro);
            DSeguroTappedCommand = new AsyncRelayCommand(QuitarSeguro);
        }

        private async Task ActivarSeguro()
        {
            try
            {
                var filtro = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse("67ed8c3d1f95e236d4cff150"));
                var actualizacion = Builders<BsonDocument>.Update
                    .Set("Fecha", DateTime.UtcNow)
                    .Set("Estado", true);

                var resultado = await _circuitoCollection.UpdateOneAsync(filtro, actualizacion);

                if (resultado.ModifiedCount > 0)
                {
                    _puertaAbierta = true;
                    EstadoSeguro = "Seguro: Activado";
                    SeguroActivado = true;
                    SeguroDesactivado = false;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Advertencia", "No se actualizó ningún documento.", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Error al activar el seguro: {ex.Message}", "OK");
            }
        }
        private async Task QuitarSeguro()
        {
            try
            {
                var filtro = Builders<BsonDocument>.Filter.Eq("_id", ObjectId.Parse("67ed8c3d1f95e236d4cff150"));
                var actualizacion = Builders<BsonDocument>.Update
                    .Set("Fecha", DateTime.UtcNow)
                    .Set("Estado", false);

                var resultado = await _circuitoCollection.UpdateOneAsync(filtro, actualizacion);
                if (resultado.ModifiedCount > 0)
                {
                    _puertaAbierta = true;
                    EstadoSeguro = "Seguro: Desactivado";
                    SeguroActivado = true;
                    SeguroDesactivado = false;
                }
                else
                {
                    await App.Current.MainPage.DisplayAlert("Advertencia", "No se actualizó ningún documento.", "OK");
                }
            }
            catch (Exception ex)
            {
                await App.Current.MainPage.DisplayAlert("Error", $"Error al activar el seguro: {ex.Message}", "OK");
            }
        }
    }
}
