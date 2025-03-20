using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Interfases.VistaModel
{
    public class AcesoRemotoVM : INotifyPropertyChanged
    {
        private bool puertaAbierta;
        private bool seguroActivado;
        private int tiempoRestante;
        private System.Timers.Timer temporizador;

        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Historial { get; set; } = new ObservableCollection<string>();

        public bool PuertaCerrada => !puertaAbierta;
        public bool PuertaAbierta => puertaAbierta;
        public string EstadoPuerta => puertaAbierta ? "Puerta Abierta" : "Puerta Cerrada";
        public string EstadoSeguro => seguroActivado ? "Seguro Activado" : "Seguro Desactivado";
        public bool SeguroActivado => seguroActivado;
        public bool SeguroDesactivado => !seguroActivado;
        public bool MostrarContador => tiempoRestante > 0;
        public string Contador => $" {tiempoRestante} s";

        public ICommand PuertaTappedCommand { get; }
        public ICommand SeguroTappedCommand { get; }

        public AcesoRemotoVM()
        {
            PuertaTappedCommand = new Command(async () => await OnPuertaTapped());
            SeguroTappedCommand = new Command(OnSeguroTapped);
        }

        private async Task OnPuertaTapped()
        {
            if (seguroActivado)
            {
                await Application.Current.MainPage.DisplayAlert("Acceso Denegado", "La puerta tiene el seguro activado.", "OK");
                return;
            }

            if (!puertaAbierta)
            {
                puertaAbierta = true;
                tiempoRestante = 10;
                Historial.Add($"Puerta abierta a las {DateTime.Now.ToShortTimeString()}");
                StartTimer();
            }
            else
            {
                puertaAbierta = false;
                temporizador?.Stop();
                Historial.Add($"Puerta cerrada manualmente a las {DateTime.Now.ToShortTimeString()}");
            }

            NotifyPropertyChanged(nameof(PuertaAbierta));
            NotifyPropertyChanged(nameof(PuertaCerrada));
            NotifyPropertyChanged(nameof(EstadoPuerta));
            NotifyPropertyChanged(nameof(MostrarContador));
        }

        private void OnSeguroTapped()
        {
            seguroActivado = !seguroActivado;
            Historial.Add($"{(seguroActivado ? "Seguro activado" : "Seguro desactivado")} {DateTime.Now.ToShortTimeString()}");
            NotifyPropertyChanged(nameof(SeguroActivado));
            NotifyPropertyChanged(nameof(SeguroDesactivado));
            NotifyPropertyChanged(nameof(EstadoSeguro));
        }

        [Obsolete]
        private void StartTimer()
        {
            temporizador = new System.Timers.Timer(1000);
            temporizador.Elapsed += (s, e) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    tiempoRestante--;
                    NotifyPropertyChanged(nameof(Contador));

                    if (tiempoRestante == 0)
                    {
                        puertaAbierta = false;
                        NotifyPropertyChanged(nameof(PuertaAbierta));
                        NotifyPropertyChanged(nameof(PuertaCerrada));
                        NotifyPropertyChanged(nameof(EstadoPuerta));
                        NotifyPropertyChanged(nameof(MostrarContador));
                        temporizador.Stop();
                    }
                });
            };
            temporizador.Start();
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
