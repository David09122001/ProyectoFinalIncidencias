using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ProjecteFinal.ViewModels
{
    public class DetalleIncidenciaVM : INotifyPropertyChanged
    {
        public Incidencia Incidencia { get; set; }
        public ObservableCollection<Adjunto> Adjuntos { get; set; } = new ObservableCollection<Adjunto>();
        public Incidencia_HW IncidenciaHW { get;  set; }
        public Incidencia_SW IncidenciaSW { get;  set; }
        public Incidencia_Red IncidenciaRed { get;  set; }

        public AdjuntoDAO AdjuntoDAO { get; private set; }

        public ObservableCollection<string> Tipos { get; set; } = new ObservableCollection<string> { "Hardware", "Software", "Red" };

        private string _tipoSeleccionado;
        public string TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                if (_tipoSeleccionado != value)
                {
                    _ = ConfirmarCambioTipoAsync(value);
                }
            }
        }

        public bool MostrarHW => IncidenciaHW != null;
        public bool MostrarSW => IncidenciaSW != null;
        public bool MostrarRed => IncidenciaRed != null;


        private readonly IncidenciaHWDAO incidenciaHWDAO = new IncidenciaHWDAO();
        private readonly IncidenciaSWDAO incidenciaSWDAO = new IncidenciaSWDAO();
        private readonly IncidenciaRedDAO incidenciaRedDAO = new IncidenciaRedDAO();

        public Profesor Profesor { get;  set; } 

        private ProfesorDAO profesorDAO = new ProfesorDAO();
        public DetalleIncidenciaVM(Incidencia incidencia)
        {
            Incidencia = incidencia;
            AdjuntoDAO = new AdjuntoDAO();  
            TipoSeleccionado = Incidencia.estado; // Inicializar según el estado de la incidencia
            CargarDatosAsync();
        }

        private async Task CargarDatosAsync()
        {
            await CargarDatosSubtipoAsync();
            await CargarAdjuntosAsync();
            await CargarProfesorAsync();
        }
        private async Task ConfirmarCambioTipoAsync(string nuevoTipo)
        {
            _tipoSeleccionado = nuevoTipo;
            await CargarDatosSubtipoAsync();
        }

        private async Task CargarProfesorAsync()
        {
            if (!string.IsNullOrEmpty(Incidencia.profesorDni))
            {
                Profesor = await profesorDAO.BuscarPorDniAsync(Incidencia.profesorDni);
                OnPropertyChanged(nameof(Profesor)); 
            }
        }

        private async Task CargarAdjuntosAsync()
        {
            var adjuntos = await AdjuntoDAO.ObtenerAdjuntosPorIncidenciaAsync(Incidencia.id);
            Adjuntos.Clear();
            foreach (var adjunto in adjuntos)
            {
                Adjuntos.Add(adjunto);
            }
        }


        private async Task CargarDatosSubtipoAsync()
        {
            // Intentar cargar una especialización de Hardware
            IncidenciaHW = await incidenciaHWDAO.ObtenerIncidenciaHWPorIdAsync(Incidencia.id);
            if (IncidenciaHW != null)
            {
                TipoSeleccionado = "Hardware";
            }
            else
            {
                // Intentar cargar una especialización de Software
                IncidenciaSW = await incidenciaSWDAO.ObtenerIncidenciaSWPorId(Incidencia.id);
                if (IncidenciaSW != null)
                {
                    TipoSeleccionado = "Software";
                }
                else
                {
                    // Intentar cargar una especialización de Red
                    IncidenciaRed = await incidenciaRedDAO.ObtenerIncidenciaRedPorId(Incidencia.id);
                    if (IncidenciaRed != null)
                    {
                        TipoSeleccionado = "Red";
                    }
                }
            }

            // Notificar cambios a las propiedades enlazadas
            OnPropertyChanged(nameof(IncidenciaHW));
            OnPropertyChanged(nameof(IncidenciaSW));
            OnPropertyChanged(nameof(IncidenciaRed));
            OnPropertyChanged(nameof(MostrarHW));
            OnPropertyChanged(nameof(MostrarSW));
            OnPropertyChanged(nameof(MostrarRed));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
