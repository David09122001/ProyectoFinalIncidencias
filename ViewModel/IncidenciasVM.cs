using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using ProjecteFinal.Views;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ProjecteFinal.ViewModel
{
    public class IncidenciasVM : INotifyPropertyChanged
    {
        private IncidenciaDAO incidenciaDAO;
        private ProfesorDAO profesorDAO;

        public ObservableCollection<Incidencia> Incidencias { get; private set; }

        private Incidencia _selectedIncidencia;
        public Incidencia SelectedIncidencia
        {
            get => _selectedIncidencia;
            set
            {
                _selectedIncidencia = value;
                OnPropertyChanged(nameof(SelectedIncidencia));
            }
        }

        private string _profesorNombre;
        public string ProfesorNombre
        {
            get => _profesorNombre;
            set
            {
                _profesorNombre = value;
                OnPropertyChanged(nameof(ProfesorNombre));
            }
        }
        public IncidenciasVM()
        {
            incidenciaDAO = new IncidenciaDAO();
            profesorDAO = new ProfesorDAO();
            Incidencias = new ObservableCollection<Incidencia>();
            CargarIncidenciasAsync();
        }


        public async Task CargarIncidenciasAsync()
        {
            Incidencias.Clear();
            var listaIncidencias = incidenciaDAO.ObtenerIncidencias();

            foreach (var incidencia in listaIncidencias)
            {
                // Obtener el nombre del profesor responsable
                if (!string.IsNullOrWhiteSpace(incidencia.responsableDni))
                {
                    var profesor = await profesorDAO.BuscarPorDniAsync(incidencia.responsableDni);
                    incidencia.responsableDni = profesor?.nombre ?? "Ningún profesor asignado";
                }
                else
                {
                    incidencia.responsableDni = "Ningún profesor asignado";
                }

                Incidencias.Add(incidencia);
            }
        }




        public void AñadirIncidencia(Incidencia nuevaIncidencia)
        {
            incidenciaDAO.AñadirIncidenciaAsync(nuevaIncidencia);
            Incidencias.Add(nuevaIncidencia);
        }

        public void ActualizarIncidencia(Incidencia incidenciaModificada)
        {
            incidenciaDAO.ActualizarIncidencia(incidenciaModificada);

            var incidenciaExistente = Incidencias.FirstOrDefault(i => i.id == incidenciaModificada.id);
            if (incidenciaExistente != null)
            {
                incidenciaExistente.descripcionDetallada = incidenciaModificada.descripcionDetallada;
                incidenciaExistente.aulaUbicacion = incidenciaModificada.aulaUbicacion;
                incidenciaExistente.fechaIncidencia = incidenciaModificada.fechaIncidencia;
                incidenciaExistente.observaciones = incidenciaModificada.observaciones;
                incidenciaExistente.tiempoInvertido = incidenciaModificada.tiempoInvertido;
                incidenciaExistente.fechaResolucion = incidenciaModificada.fechaResolucion;
            }
        }

        public bool EliminarIncidencia(Incidencia incidencia)
        {
            try
            {
                incidenciaDAO.EliminarIncidencia(incidencia);
                Incidencias.Remove(incidencia);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}