using ProjecteFinal.Models;
using ProjecteFinal.DAO;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using ProjecteFinal.Base;
using System.Windows.Input;

namespace ProjecteFinal.ViewModel
{
    public class InsertarIncidenciaVM : BaseViewModel
    {
        private IncidenciaDAO incidenciaDAO = new IncidenciaDAO();
        private IncidenciaHWDAO incidenciaHWDAO = new IncidenciaHWDAO();
        private IncidenciaSWDAO incidenciaSWDAO = new IncidenciaSWDAO();
        private IncidenciaRedDAO incidenciaRedDAO = new IncidenciaRedDAO();
        private AdjuntoDAO adjuntoDAO = new AdjuntoDAO();

        public ObservableCollection<Adjunto> Adjuntos { get; set; } = new ObservableCollection<Adjunto>();
        public ObservableCollection<string> Tipos { get; set; } = new ObservableCollection<string> { "Hardware", "Software", "Red" };

        private string _tipoSeleccionado;
        public string TipoSeleccionado
        {
            get => _tipoSeleccionado;
            set
            {
                _tipoSeleccionado = value;
                OnPropertyChanged();
                ActualizarFormulario();
            }
        }

        public Incidencia Incidencia { get; set; } = new Incidencia();
        public Incidencia_HW IncidenciaHW { get; set; } = new Incidencia_HW(); 
        public Incidencia_SW IncidenciaSW { get; set; } = new Incidencia_SW(); 
        public Incidencia_Red IncidenciaRed { get; set; } = new Incidencia_Red();

        public bool MostrarHW { get; set; }
        public bool MostrarSW { get; set; }
        public bool MostrarRed { get; set; }
        public ICommand EliminarAdjuntoCommand { get; }

        public InsertarIncidenciaVM(Profesor profesor)
        {
            Incidencia.profesorDni = profesor.dni;
            Incidencia.fechaIncidencia = DateTime.Now;
            EliminarAdjuntoCommand = new Command<Adjunto>(EliminarAdjunto);
        }

        private async void EliminarAdjunto(Adjunto adjunto)
        {
            if (Adjuntos.Contains(adjunto))
            {
                Adjuntos.Remove(adjunto);

                // Eliminar de la base de datos
                try
                {
                    await adjuntoDAO.EliminarAdjuntoAsync(adjunto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al eliminar el adjunto: {ex.Message}");
                }
            }
        }

            private void ActualizarFormulario()
        {
            MostrarHW = TipoSeleccionado == "Hardware";
            MostrarSW = TipoSeleccionado == "Software";
            MostrarRed = TipoSeleccionado == "Red";
            OnPropertyChanged(nameof(MostrarHW));
            OnPropertyChanged(nameof(MostrarSW));
            OnPropertyChanged(nameof(MostrarRed));
        }

        public async Task<bool> GuardarIncidenciaAsync()
        {
            // Validar campos obligatorios generales
            if (string.IsNullOrWhiteSpace(Incidencia.descripcionDetallada) || string.IsNullOrWhiteSpace(Incidencia.aulaUbicacion))
            {
                throw new InvalidOperationException("Completa todos los campos obligatorios.");
            }

            // Validar tipo de incidencia
            if (string.IsNullOrWhiteSpace(TipoSeleccionado))
            {
                throw new InvalidOperationException("Selecciona un tipo de incidencia (Hardware, Software o Red).");
            }

            try
            {
                // Asignar estado predeterminado
                Incidencia.estado = "Comunicada";

                // Guardar incidencia principal
                await incidenciaDAO.AñadirIncidenciaAsync(Incidencia);

                // Validar y guardar subtipo
                if (TipoSeleccionado == "Hardware")
                {
                    if (string.IsNullOrWhiteSpace(IncidenciaHW.dispositivo))
                    {
                        throw new InvalidOperationException("Completa los datos específicos de Hardware.");
                    }

                    IncidenciaHW.id = Incidencia.id;
                    await incidenciaHWDAO.AñadirIncidenciaHWAsync(IncidenciaHW);
                }
                else if (TipoSeleccionado == "Software")
                {
                    if (string.IsNullOrWhiteSpace(IncidenciaSW.sistemaOperativo))
                    {
                        throw new InvalidOperationException("Completa los datos específicos de Software.");
                    }

                    IncidenciaSW.id = Incidencia.id;
                    await incidenciaSWDAO.AñadirIncidenciaSWAsync(IncidenciaSW);
                }
                else if (TipoSeleccionado == "Red")
                {
                    if (string.IsNullOrWhiteSpace(IncidenciaRed.dispositivoAfectado))
                    {
                        throw new InvalidOperationException("Completa los datos específicos de Red.");
                    }

                    IncidenciaRed.id = Incidencia.id;
                    await incidenciaRedDAO.AñadirIncidenciaRedAsync(IncidenciaRed);
                }

                // Guardar adjuntos
                foreach (var adjunto in Adjuntos)
                {
                    adjunto.IncidenciaId = Incidencia.id;
                    await adjuntoDAO.AñadirAdjuntoAsync(adjunto);
                }

                return true; // Éxito
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error al guardar la incidencia: {ex.Message}");
            }
        }



        public void AgregarAdjunto(string nombre, string ruta, byte[] datos)
        {
            Adjuntos.Add(new Adjunto { Nombre = nombre, Ruta = ruta, Datos = datos });
        }
    }
}
