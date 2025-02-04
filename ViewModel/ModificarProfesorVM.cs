using ProjecteFinal.DAO;
using ProjecteFinal.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProjecteFinal.ViewModel
{
    public class ModificarProfesorVM : INotifyPropertyChanged
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


        public ModificarProfesorVM(Profesor profesor = null)
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

            var profesorActualizado = await profesorDAO.BuscarPorDniAsync(Profesor.dni);
            if (profesorActualizado != null)
            {
                Profesor.nombre = profesorActualizado.nombre;
                Profesor.email = profesorActualizado.email;
                Profesor.departamentoCodigo = profesorActualizado.departamentoCodigo;
                Profesor.rol_id = profesorActualizado.rol_id;
                Profesor.contrasena = profesorActualizado.contrasena;
            }

            SelectedDepartamento = Departamentos.FirstOrDefault(d => d.codigo == Profesor.departamentoCodigo);
            SelectedRol = Roles.FirstOrDefault(r => r.id == Profesor.rol_id);

            OnPropertyChanged(nameof(Profesor));
            OnPropertyChanged(nameof(SelectedDepartamento));
            OnPropertyChanged(nameof(SelectedRol));
            OnPropertyChanged(nameof(Departamentos));
            OnPropertyChanged(nameof(Roles));
        }

        private async Task<bool> ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El email es incorrecto.", "Aceptar");
                return false;
            }

            if (!email.Contains("@") || email.IndexOf("@") == 0)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El email es incorrecto.", "Aceptar");
                return false;
            }

            if (!email.EndsWith("@edu.gva"))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El email es incorrecto.", "Aceptar");
                return false;
            }

            var profesorExistente = await profesorDAO.ObtenerProfesorPorCorreoAsync(email);

            // Si el email ya existe pero es el mismo del profesor actual, no hay problema
            if (profesorExistente != null && profesorExistente.dni != Profesor.dni)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "El email ya está en uso.", "Aceptar");
                return false;
            }

            return true;
        }



        public async Task<bool> GuardarProfesorAsync()
        {
            try
            {

                if (Profesor == null || string.IsNullOrWhiteSpace(Profesor.dni) ||
                    string.IsNullOrWhiteSpace(Profesor.nombre) ||
                    string.IsNullOrWhiteSpace(Profesor.email) ||
                    Profesor.departamentoCodigo == null || Profesor.rol_id <= 0)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Todos los campos son obligatorios.", "Aceptar");
                    return false;
                }

                // Validación del email
                if (!await ValidarEmail(Profesor.email))
                {
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

                await profesorDAO.InsertarProfesorAsync(Profesor);
                return true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Ocurrió un error al guardar el profesor.", "Aceptar");
                return false;
            }
        }



        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
