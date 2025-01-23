using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ProjecteFinal.ViewModels
{
    public class InsertarModificarProfesorVM : INotifyPropertyChanged
    {
        private readonly ProfesorDAO profesorDAO;
        private readonly DepartamentoDAO departamentoDAO;
        private readonly RolDAO rolDAO;

        public Profesor Profesor { get; set; }
        public ObservableCollection<Departamento> Departamentos { get; private set; }
        public ObservableCollection<Rol> Roles { get; private set; }

        public bool IsDniEditable { get; private set; }

        private Departamento _selectedDepartamento;
        public Departamento SelectedDepartamento
        {
            get => _selectedDepartamento;
            set
            {
                if (_selectedDepartamento != value)
                {
                    _selectedDepartamento = value;
                    Profesor.departamentoCodigo = _selectedDepartamento?.codigo;
                    OnPropertyChanged(nameof(SelectedDepartamento));
                }
            }
        }

        private Rol _selectedRol;
        public Rol SelectedRol
        {
            get => _selectedRol;
            set
            {
                if (_selectedRol != value)
                {
                    _selectedRol = value;
                    Profesor.rol_id = _selectedRol?.id ?? 0;
                    OnPropertyChanged(nameof(SelectedRol));
                }
            }
        }



        public InsertarModificarProfesorVM(Profesor profesor = null)
        {
            profesorDAO = new ProfesorDAO();
            departamentoDAO = new DepartamentoDAO();
            rolDAO = new RolDAO();

            Profesor = profesor ?? new Profesor();
            IsDniEditable = profesor == null; // Editable solo si es nuevo

            Departamentos = new ObservableCollection<Departamento>();
            Roles = new ObservableCollection<Rol>();

            CargarDatosAsync();
        }

        private async void CargarDatosAsync()
        {
            var departamentos = await departamentoDAO.BuscarTodosAsync();
            var roles = await rolDAO.ObtenerRolesAsync();

            Departamentos.Clear();
            foreach (var departamento in departamentos)
            {
                Departamentos.Add(departamento);
            }

            Roles.Clear();
            foreach (var rol in roles)
            {
                Roles.Add(rol);
            }

            // Sincronizar valores seleccionados
            SelectedDepartamento = Departamentos.FirstOrDefault(d => d.codigo == Profesor.departamentoCodigo);
            SelectedRol = Roles.FirstOrDefault(r => r.id == Profesor.rol_id);

            OnPropertyChanged(nameof(Departamentos));
            OnPropertyChanged(nameof(Roles));
        }

        public async Task<bool> GuardarProfesorAsync()
        {
            try
            {
                Console.WriteLine($"Departamento seleccionado: {Profesor.departamentoCodigo}");
                Console.WriteLine($"Rol seleccionado: {Profesor.rol_id}");

                if (Profesor == null || string.IsNullOrWhiteSpace(Profesor.dni) ||
                    string.IsNullOrWhiteSpace(Profesor.nombre) ||
                    string.IsNullOrWhiteSpace(Profesor.email) ||
                    string.IsNullOrWhiteSpace(Profesor.contrasena) ||
                    Profesor.departamentoCodigo == null || Profesor.rol_id <= 0)
                {
                    return false; // Datos inválidos
                }

                await profesorDAO.InsertarProfesorAsync(Profesor);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el profesor: {ex.Message}");
                return false;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
