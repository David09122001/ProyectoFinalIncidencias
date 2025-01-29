using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
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


        public InsertarModificarProfesorVM(Profesor profesor = null)
        {
            profesorDAO = new ProfesorDAO();
            departamentoDAO = new DepartamentoDAO();
            rolDAO = new RolDAO();

            Profesor = profesor ?? new Profesor();
            IsDniEditable = profesor == null; 

            if (profesor != null)
            {
                ConfirmarContrasena = profesor.contrasena;
            }


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

        // Validación de correo electrónico
        private bool ValidarEmail(string email)
        {
            return email.EndsWith("@edu.gva");
        }

        // Validación de DNI
        private bool ValidarDNI(string dni)
        {
            var dniPattern = @"^\d{8}[A-Za-z]$";  // 8 números + 1 letra
            if (!Regex.IsMatch(dni, dniPattern))
                return false;

            // Cálculo de la letra del DNI
            string letras = "TRWAGMYFPDXBNJZSQVHLCKE";
            int numero = int.Parse(dni.Substring(0, 8));
            char letraCalculada = letras[numero % 23];

            // Verifica si la letra calculada coincide con la letra del DNI
            return dni[8].ToString().ToUpper() == letraCalculada.ToString();
        }

        // Método de guardado
        public async Task<bool> GuardarProfesorAsync()
        {
            try
            {
                Console.WriteLine($"Departamento seleccionado: {Profesor.departamentoCodigo}");
                Console.WriteLine($"Rol seleccionado: {Profesor.rol_id}");

                // Validación de campos obligatorios
                if (Profesor == null || string.IsNullOrWhiteSpace(Profesor.dni) ||
                    string.IsNullOrWhiteSpace(Profesor.nombre) ||
                    string.IsNullOrWhiteSpace(Profesor.email) ||
                    Profesor.departamentoCodigo == null || Profesor.rol_id <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Todos los campos son obligatorios.", "Aceptar");
                    return false;
                }

                // Validación del correo electrónico
                if (!ValidarEmail(Profesor.email))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "El correo electrónico debe terminar con '@edu.gva'", "Aceptar");
                    return false;
                }

                // Validación del DNI
                if (!ValidarDNI(Profesor.dni))
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "El DNI es incorrecto. Debe tener 8 números y una letra válida.", "Aceptar");
                    return false;
                }

                // Validación de contraseñas
                if (!string.IsNullOrWhiteSpace(Profesor.contrasena) || !string.IsNullOrWhiteSpace(ConfirmarContrasena))
                {
                    if (Profesor.contrasena != ConfirmarContrasena)
                    {
                        await Application.Current.MainPage.DisplayAlert("Error", "Las contraseñas no coinciden.", "Aceptar");
                        return false;
                    }
                }

                // Guardar el profesor
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
