using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace GestorIncidencias.Models
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
        private string _estado;
        [NotNull]
        public string estado
        {
            get => _estado;
            set
            {
                if (_estado != value)
                {
                    _estado = value;
                    OnPropertyChanged(nameof(estado));
                }
            }
        }


        private DateTime? _fechaResolucion;
        public DateTime? fechaResolucion
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

       
        [ForeignKey(typeof(Profesor)), NotNull]
        public string profesorDni { get; set; }

        private string _responsableDni;
        [ForeignKey(typeof(Profesor))]
        public string responsableDni
        {
            get => _responsableDni;
            set
            {
                if (_responsableDni != value)
                {
                    _responsableDni = value;
                    OnPropertyChanged(nameof(responsableDni));
                }
            }
        }


        private string _nombreResponsable;

        [Ignore] 
        public string NombreResponsable
        {
            get => _nombreResponsable;
            set
            {
                if (_nombreResponsable != value) 
                {
                    _nombreResponsable = value;
                    OnPropertyChanged(nameof(NombreResponsable)); 
                }
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}