using SQLite;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjecteFinal.Models
{
    public class Permiso : INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int codigo { get; set; }

        [MaxLength(100), NotNull]
        public string descripcion { get; set; }

        
        private bool _isAssigned;
        [Ignore]
        public bool IsAssigned
        {
            get => _isAssigned;
            set
            {
                _isAssigned = value;
                OnPropertyChanged(nameof(IsAssigned));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
