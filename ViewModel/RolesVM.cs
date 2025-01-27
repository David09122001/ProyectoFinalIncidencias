using ProjecteFinal.Models;
using ProjecteFinal.DAO;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using ProjecteFinal.Base;

namespace ProjecteFinal.ViewModel
{
    public class RolesVM : BaseViewModel
    {
        private readonly RolDAO rolDAO;
        private readonly PermisoDAO permisoDAO;

        // Roles
        private ObservableCollection<Rol> _roles;
        public ObservableCollection<Rol> Roles
        {
            get => _roles;
            set
            {
                _roles = value;
                OnPropertyChanged();
                FiltrarRoles();
            }
        }

        private ObservableCollection<Rol> _rolesFiltrados;
        public ObservableCollection<Rol> RolesFiltrados
        {
            get => _rolesFiltrados;
            set
            {
                _rolesFiltrados = value;
                OnPropertyChanged();
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
                    FiltrarRoles();
                }
            }
        }

        private ObservableCollection<Permiso> _permisosDisponibles;
        public ObservableCollection<Permiso> PermisosDisponibles
        {
            get => _permisosDisponibles;
            set
            {
                _permisosDisponibles = value;
                OnPropertyChanged();
            }
        }



        private Rol _rolSeleccionado;
        public Rol RolSeleccionado
        {
            get => _rolSeleccionado;
            set
            {
                _rolSeleccionado = value;
                OnPropertyChanged();
                if (_rolSeleccionado != null)
                {
                    _ = CargarPermisosAsync(_rolSeleccionado.id);
                }
            }
        }

        // Constructor
        public RolesVM()
        {
            rolDAO = new RolDAO();
            permisoDAO = new PermisoDAO();

            Roles = new ObservableCollection<Rol>();
            RolesFiltrados = new ObservableCollection<Rol>();
            PermisosDisponibles = new ObservableCollection<Permiso>();

            _ = CargarRolesAsync();
        }


        // Métodos
        public async Task CargarRolesAsync()
        {
            var roles = await rolDAO.ObtenerRolesAsync();
            Roles = new ObservableCollection<Rol>(roles);
        }

        public async Task CargarPermisosAsync(int rolId)
        {
            PermisosDisponibles.Clear();

            var todosLosPermisos = await permisoDAO.ObtenerPermisosAsync();
            var permisosAsignados = (await permisoDAO.ObtenerPermisosPorRolAsync(rolId))
                .Select(p => p.codigo)
                .ToList();

            foreach (var permiso in todosLosPermisos)
            {
                permiso.IsAssigned = permisosAsignados.Contains(permiso.codigo);
                PermisosDisponibles.Add(permiso);
            }
        }

        public async Task GuardarRolAsync()
        {
            if (RolSeleccionado == null)
                throw new ArgumentException("El rol no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(RolSeleccionado.nombre))
                throw new ArgumentException("El nombre del rol es obligatorio.");

            // Guardamos el rol
            await rolDAO.GuardarRolAsync(RolSeleccionado);

            // Guardamos los permisos asignados
            var permisosSeleccionados = PermisosDisponibles
                .Where(p => p.IsAssigned)
                .Select(p => p.codigo)
                .ToList();

            await permisoDAO.EliminarPermisosPorRolAsync(RolSeleccionado.id);

            foreach (var permisoCodigo in permisosSeleccionados)
            {
                var rolPermiso = new RolPermiso
                {
                    rolId = RolSeleccionado.id,
                    permisoCodigo = permisoCodigo
                };
                await permisoDAO.InsertarRolPermisoAsync(rolPermiso);
            }
        }

        public async Task EliminarRolAsync(Rol rol)
        {
            if (rol == null)
                throw new ArgumentException("No se puede eliminar un rol nulo.");

            await rolDAO.EliminarRolAsync(rol);
            Roles.Remove(rol);
            RolesFiltrados.Remove(rol);
        }

        public void FiltrarRoles()
        {
            if (RolesFiltrados == null)
            {
                RolesFiltrados = new ObservableCollection<Rol>();
            }

            RolesFiltrados.Clear();

            var resultados = string.IsNullOrWhiteSpace(TextoBusqueda)
                ? Roles
                : Roles.Where(r => r.nombre.ToLower().Contains(TextoBusqueda.ToLower()));

            foreach (var rol in resultados)
            {
                RolesFiltrados.Add(rol);
            }
        }
    }
}
