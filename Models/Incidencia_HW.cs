using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjecteFinal.Models
{
    public class Incidencia_HW : INotifyPropertyChanged
    {
        [PrimaryKey, ForeignKey(typeof(Incidencia))]
        public int id { get; set; }

        private string _dispositivo;
        [MaxLength(50), NotNull]
        public string dispositivo
        {
            get => _dispositivo;
            set
            {
                if (_dispositivo != value)
                {
                    _dispositivo = value;
                    OnPropertyChanged(nameof(dispositivo));
                }
            }
        }

        public string modelo { get; set; }
        public string numeroSerie { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}