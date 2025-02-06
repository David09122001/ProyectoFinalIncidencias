using GestorIncidencias.DAO;
using GestorIncidencias.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace GestorIncidencias.ViewModels
{
    public class ProfesoresVM : INotifyPropertyChanged
    {
        private readonly ProfesorDAO profesorDAO;
        private readonly DepartamentoDAO departamentoDAO;

        public ObservableCollection<Profesor> Profesores { get; set; }
        public ObservableCollection<Profesor> ProfesoresFiltrados { get; set; }
        public ObservableCollection<string> Departamentos { get; set; }

        private string _textoBusqueda;
        public string TextoBusqueda
        {
            get => _textoBusqueda;
            set
            {
                _textoBusqueda = value;
                OnPropertyChanged(nameof(TextoBusqueda));
                AplicarFiltros();
            }
        }

        public ProfesoresVM()
        {
            profesorDAO = new ProfesorDAO();
            departamentoDAO = new DepartamentoDAO();

            Profesores = new ObservableCollection<Profesor>();
            ProfesoresFiltrados = new ObservableCollection<Profesor>();
            Departamentos = new ObservableCollection<string>();

            CargarDatos();
        }

        private async void CargarDatos()
        {
            var profesores = await profesorDAO.BuscarTodosAsync();
            Profesores.Clear();

            foreach (var profesor in profesores)
            {
                Profesores.Add(profesor);
            }

            AplicarFiltros();

            var departamentos = await departamentoDAO.BuscarTodosAsync();
            Departamentos.Clear();

            foreach (var departamento in departamentos)
            {
                Departamentos.Add(departamento.nombre);
            }
        }

        public bool EliminarProfesor(Profesor profesor)
        {
            try
            {
                profesorDAO.EliminarProfesor(profesor);
                Profesores.Remove(profesor);
                ProfesoresFiltrados.Remove(profesor);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void AplicarFiltros()
        {
            var filtrado = Profesores.ToList();

            if (!string.IsNullOrWhiteSpace(TextoBusqueda))
            {
                filtrado = filtrado
                    .Where(p => p.nombre.Contains(TextoBusqueda, System.StringComparison.OrdinalIgnoreCase) ||
                                p.email.Contains(TextoBusqueda, System.StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            ProfesoresFiltrados.Clear();
            foreach (var profesor in filtrado)
            {
                ProfesoresFiltrados.Add(profesor);
            }
        }

        public async Task RecargarDatos()
        {
            var profesores = await profesorDAO.BuscarTodosAsync();
            Profesores.Clear();

            foreach (var profesor in profesores)
            {
                Profesores.Add(profesor);
            }

            AplicarFiltros(); 
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
