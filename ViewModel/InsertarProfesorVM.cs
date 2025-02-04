using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjecteFinal.ViewModel
{
    public class InsertarProfesorVM : INotifyPropertyChanged
    {
        private readonly ProfesorDAO profesorDAO;
        private readonly DepartamentoDAO departamentoDAO;
        private readonly RolDAO rolDAO;

        public Profesor Profesor { get; set; } = new Profesor();

        public ObservableCollection<Departamento> Departamentos { get; private set; }
        public ObservableCollection<Rol> Roles { get; private set; }

        private Departamento _selectedDepartamento;
        public Departamento SelectedDepartamento
        {
            get => _selectedDepartamento;
            set
            {
                if (_selectedDepartamento != value)
                {
                    _selectedDepartamento = value;
                    Profesor.departamentoCodigo = _selectedDepartamento?.codigo ?? string.Empty;
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

        private string _confirmarContrasena;
        public string ConfirmarContrasena
        {
            get => _confirmarContrasena;
            set
            {
                if (_confirmarContrasena != value)
                {
                    _confirmarContrasena = value;
                    OnPropertyChanged(nameof(ConfirmarContrasena));
                }
            }
        }

        public InsertarProfesorVM()
        {
            profesorDAO = new ProfesorDAO();
            departamentoDAO = new DepartamentoDAO();
            rolDAO = new RolDAO();

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

            OnPropertyChanged(nameof(Departamentos));
            OnPropertyChanged(nameof(Roles));
        }

        private async Task<bool> ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email) || !email.Contains("@") || email.IndexOf("@") == 0 || !email.EndsWith("@edu.gva"))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El email es incorrecto.", "Aceptar");
                return false;
            }

            var profesorExistente = await profesorDAO.ObtenerProfesorPorCorreoAsync(email);
            if (profesorExistente != null)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El email ya está en uso.", "Aceptar");
                return false;
            }

            return true;
        }

        private bool ValidarDNI(string dni)
        {
            var dniPattern = @"^\d{8}[A-Za-z]$";
            if (!Regex.IsMatch(dni, dniPattern))
                return false;

            string letras = "TRWAGMYFPDXBNJZSQVHLCKE";
            int numero = int.Parse(dni.Substring(0, 8));
            char letraCalculada = letras[numero % 23];

            return dni[8].ToString().ToUpper() == letraCalculada.ToString();
        }

        public async Task<bool> GuardarProfesorAsync()
        {
            try
            {
                if (Profesor == null || string.IsNullOrWhiteSpace(Profesor.dni) ||
                    string.IsNullOrWhiteSpace(Profesor.nombre) ||
                    string.IsNullOrWhiteSpace(Profesor.email) ||
                    string.IsNullOrWhiteSpace(Profesor.departamentoCodigo) ||
                    Profesor.rol_id <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Todos los campos son obligatorios.", "Aceptar");
                    return false;
                }

                if (!await ValidarEmail(Profesor.email))
                {
                    return false;
                }

                var profesorExistente = await profesorDAO.BuscarPorDniAsync(Profesor.dni);
                if (profesorExistente != null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Ya existe un profesor con este DNI.", "Aceptar");
                    return false;
                }

                if (!ValidarDNI(Profesor.dni))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "El DNI es incorrecto. Debe tener 8 números y una letra válida.", "Aceptar");
                    return false;
                }

                if (!string.IsNullOrWhiteSpace(Profesor.contrasena) || !string.IsNullOrWhiteSpace(ConfirmarContrasena))
                {
                    if (Profesor.contrasena != ConfirmarContrasena)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden.", "Aceptar");
                        return false;
                    }
                }

                await profesorDAO.InsertarProfesorAsync(Profesor);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar el profesor: {ex.Message}");
                await Application.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al guardar el profesor.", "Aceptar");
                return false;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
