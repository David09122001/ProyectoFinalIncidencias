using ProjecteFinal.Base;
using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjecteFinal.ViewModel
{
    public class TiposHWVM : BaseViewModel
    {
        private IncidenciaHWDAO incidenciaHWDAO;

        public ObservableCollection<Incidencia_HW> Tipos { get; private set; }
        public ObservableCollection<Incidencia_HW> TiposFiltrados { get; private set; }

        private string _textoBusqueda;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                if (_textoBusqueda != value)
                {
                    _textoBusqueda = value;
                    OnPropertyChanged();
                    FiltrarTipos();
                }
            }
        }

        public string NombreTipo { get; set; } 

        public TiposHWVM()
        {
            incidenciaHWDAO = new IncidenciaHWDAO();
            Tipos = new ObservableCollection<Incidencia_HW>();
            TiposFiltrados = new ObservableCollection<Incidencia_HW>();
            _ = CargarTiposAsync();
        }

        public async Task CargarTiposAsync()
        {
            var tipos = await incidenciaHWDAO.ObtenerTiposHWAsync();
            Tipos.Clear();
            foreach (var tipo in tipos)
            {
                Tipos.Add(tipo);
            }
            FiltrarTipos();
        }

        public void FiltrarTipos()
        {
            TiposFiltrados.Clear();
            var resultados = string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Tipos
                : Tipos.Where(t => t.dispositivo.ToLower().Contains(TextoBusqueda.ToLower()));

            foreach (var tipo in resultados)
            {
                TiposFiltrados.Add(tipo);
            }
        }

        public async Task GuardarTipoAsync()
        {
            if (string.IsNullOrWhiteSpace(NombreTipo))
                throw new ArgumentException("El nombre del tipo no puede estar vacío.");

            try
            {
                var duplicado = Tipos.Any(t => t.dispositivo.Equals(NombreTipo, StringComparison.OrdinalIgnoreCase));
                if (duplicado)
                {
                    throw new InvalidOperationException("Ya existe un tipo de hardware con este nombre.");
                }

                await incidenciaHWDAO.AñadirTipoHWAsync(NombreTipo);
                await CargarTiposAsync();

                NombreTipo = string.Empty;
                OnPropertyChanged(nameof(NombreTipo));
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error inesperado al guardar el tipo: {ex.Message}");
                throw;
            }
        }

        public async Task ModificarTipoAsync(Incidencia_HW tipo, string nuevoNombre)
        {
            if (string.IsNullOrWhiteSpace(nuevoNombre))
                throw new ArgumentException("El nombre no puede estar vacío.");

            if (tipo == null || tipo.id >= 0)
                throw new ArgumentException("Solo se pueden modificar tipos de hardware con IDs negativos.");

            try
            {
                await incidenciaHWDAO.ActualizarIncidenciaHWAsync(tipo, nuevoNombre);
                tipo.dispositivo = nuevoNombre;
                FiltrarTipos();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al modificar el tipo de hardware: {ex.Message}");
                throw;
            }
        }

        public async Task EliminarTipoAsync(Incidencia_HW tipo)
        {
            if (tipo == null || tipo.id >= 0)
                throw new ArgumentException("Solo se pueden eliminar tipos de hardware con IDs negativos.");

            try
            {
                await incidenciaHWDAO.EliminarIncidenciaHWAsync(tipo);
                Tipos.Remove(tipo);
                TiposFiltrados.Remove(tipo);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar el tipo de hardware: {ex.Message}");
                throw;
            }
        }
    }
}
