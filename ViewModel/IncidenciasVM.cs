using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ProjecteFinal.ViewModel
{
    public class IncidenciasVM : INotifyPropertyChanged
    {
        private IncidenciaDAO incidenciaDAO;

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

        public IncidenciasVM()
        {
            incidenciaDAO = new IncidenciaDAO();
            Incidencias = new ObservableCollection<Incidencia>();
            CargarIncidencias();
        }

        public void CargarIncidencias()
        {
            Incidencias.Clear();
            var listaIncidencias = incidenciaDAO.ObtenerIncidencias();

            foreach (var incidencia in listaIncidencias)
            {
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
