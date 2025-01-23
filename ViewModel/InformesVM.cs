using ProjecteFinal.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Windows.Input;

namespace ProjecteFinal.ViewModel
{
    public class InformesVM : INotifyPropertyChanged
    {
        public List<Incidencia> ListaIncidencias { get; set; }
        public ICommand GenerarInformeCommand { get; }

        public InformesVM()
        {
            

            GenerarInformeCommand = new Command(GenerarInforme);
        }

        private async void GenerarInforme()
        {
            try
            {
                // Define la ruta del archivo PDF
                var ruta = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "InformeDeIncidencias.pdf");

                // Generar el PDF
                //_pdfGenerator.GenerarInformeDeIncidencias(ruta, ListaIncidencias);

                // Mostrar mensaje de éxito
                await App.Current.MainPage.DisplayAlert("Éxito", $"El informe se generó correctamente en: {ruta}", "Aceptar");
            }
            catch (Exception ex)
            {
                // Manejar errores
                await App.Current.MainPage.DisplayAlert("Error", $"No se pudo generar el informe: {ex.Message}", "Aceptar");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
