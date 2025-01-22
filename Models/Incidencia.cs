using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ProjecteFinal.Models
{
    public class Incidencia : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        private DateTime _fechaIncidencia;
        [NotNull]
        public DateTime fechaIncidencia
        {
            get => _fechaIncidencia;
            set
            {
                _fechaIncidencia = value;
                OnPropertyChanged(nameof(fechaIncidencia));
            }
        }

        [NotNull]
        public DateTime fechaIntroduccion { get; set; } = DateTime.Now; //Inicializada a ahora por defecto

        private string _aulaUbicacion;
        [MaxLength(100), NotNull]
        public string aulaUbicacion
        {
            get => _aulaUbicacion;
            set
            {
                _aulaUbicacion = value;
                OnPropertyChanged(nameof(aulaUbicacion));
            }
        }

        private string _descripcionDetallada;
        [NotNull]
        public string descripcionDetallada
        {
            get => _descripcionDetallada;
            set
            {
                _descripcionDetallada = value;
                OnPropertyChanged(nameof(descripcionDetallada));
            }
        }

        private string _observaciones;
        public string observaciones
        {
            get => _observaciones;
            set
            {
                _observaciones = value;
                OnPropertyChanged(nameof(observaciones));
            }
        }

        [NotNull] // Poner enum
        public string estado { get; set; } // Puedes usar un enum para restringir valores posibles

        private DateTime _fechaResolucion;
        public DateTime fechaResolucion
        {
            get => _fechaResolucion;
            set
            {
                _fechaResolucion = value;
                OnPropertyChanged(nameof(fechaResolucion));
            }
        }

        private int _tiempoInvertido;
        public int tiempoInvertido
        {
            get => _tiempoInvertido;
            set
            {
                _tiempoInvertido = value;
                OnPropertyChanged(nameof(tiempoInvertido));
            }
        }

        [Ignore]
        public List<Adjunto> Adjuntos { get; set; } = new List<Adjunto>();

        public bool comunicada { get; set; } = false;

        [ForeignKey(typeof(Profesor)), NotNull]
        public string profesorDni { get; set; }

        [ForeignKey(typeof(Profesor))]
        public string responsableDni { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}