using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace GestorIncidencias.Models
{
    public class Departamento : INotifyPropertyChanged
    {
        private string _codigo;
        private string _nombre;
        private string _ubicacion;

        [PrimaryKey, MaxLength(10)]
        public string codigo
        {
            get => _codigo;
            set
            {
                if (_codigo != value)
                {
                    _codigo = value;
                    OnPropertyChanged();
                }
            }
        }

        [MaxLength(100), NotNull]
        public string nombre
        {
            get => _nombre;
            set
            {
                if (_nombre != value)
                {
                    _nombre = value;
                    OnPropertyChanged();
                }
            }
        }

        [MaxLength(100), NotNull]
        public string ubicacion
        {
            get => _ubicacion;
            set
            {
                if (_ubicacion != value)
                {
                    _ubicacion = value;
                    OnPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
