using ProjecteFinal.Base;
using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjecteFinal.ViewModel
{
    public class DepartamentosVM : BaseViewModel
    {
        private DepartamentoDAO departamentoDAO;

        private ObservableCollection<Departamento> _departamentos;
        public ObservableCollection<Departamento> Departamentos
        {
            get => _departamentos;
            set
            {
                _departamentos = value;
                OnPropertyChanged(); // Notificar cambios en la lista principal
                FiltrarDepartamentos(); // Actualizar la lista filtrada
            }
        }

        private ObservableCollection<Departamento> _departamentosFiltrados;
        public ObservableCollection<Departamento> DepartamentosFiltrados
        {
            get => _departamentosFiltrados;
            set
            {
                _departamentosFiltrados = value;
                OnPropertyChanged(); // Notificar cambios en la lista filtrada
            }
        }

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
                    FiltrarDepartamentos();
                }
            }
        }

        public DepartamentosVM()
        {
            departamentoDAO = new DepartamentoDAO();
            Departamentos = new ObservableCollection<Departamento>();
            DepartamentosFiltrados = new ObservableCollection<Departamento>();
            _ = CargarDepartamentosAsync();
        }


        public async Task CargarDepartamentosAsync()
        {
            var departamentos = await departamentoDAO.ObtenerDepartamentosAsync();
            Departamentos = new ObservableCollection<Departamento>(departamentos);
        }

        public void FiltrarDepartamentos()
        {
            if (DepartamentosFiltrados == null)
            {
                DepartamentosFiltrados = new ObservableCollection<Departamento>();
            }

            DepartamentosFiltrados.Clear();

            var resultados = string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Departamentos
                : Departamentos.Where(d => d.nombre.ToLower().Contains(TextoBusqueda.ToLower()));

            foreach (var departamento in resultados)
            {
                DepartamentosFiltrados.Add(departamento);
            }
        }


        public async Task EliminarDepartamentoAsync(Departamento departamento)
        {
            if (departamento == null)
                throw new ArgumentException("No se puede eliminar un departamento nulo.");

            await departamentoDAO.EliminarDepartamentoAsync(departamento);

            // Actualizar las listas
            Departamentos.Remove(departamento);
            DepartamentosFiltrados.Remove(departamento);
        }

        public async Task GuardarDepartamentoAsync(Departamento departamento)
        {
            if (departamento == null)
                throw new ArgumentException("El departamento no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(departamento.codigo) ||
                string.IsNullOrWhiteSpace(departamento.nombre) ||
                string.IsNullOrWhiteSpace(departamento.ubicacion))
            {
                throw new ArgumentException("Todos los campos son obligatorios.");
            }

            // Verificar duplicados en la base de datos
            var existente = await departamentoDAO.ObtenerDepartamentoPorCodigoAsync(departamento.codigo);
            if (existente != null && existente != departamento)
            {
                throw new InvalidOperationException("Ya existe un departamento con este código.");
            }

            // Guardar o actualizar en la base de datos
            await departamentoDAO.GuardarDepartamentoAsync(departamento);

            // Recargar la lista desde la base de datos
            await CargarDepartamentosAsync();
        }

    }
}
