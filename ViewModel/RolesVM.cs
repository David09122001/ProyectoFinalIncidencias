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

        private Rol _nuevoRol;
        public Rol NuevoRol
        {
            get => _nuevoRol;
            set
            {
                _nuevoRol = value;
                OnPropertyChanged();
            }
        }

        public RolesVM()
        {
            rolDAO = new RolDAO();
            permisoDAO = new PermisoDAO();

            Roles = new ObservableCollection<Rol>();
            RolesFiltrados = new ObservableCollection<Rol>();
            PermisosDisponibles = new ObservableCollection<Permiso>();
            NuevoRol = new Rol();
            _ = CargarRolesAsync();
        }


        public async Task CargarRolesAsync()
        {
            var roles = await rolDAO.ObtenerRolesAsync();
            Roles = new ObservableCollection<Rol>(roles);
        }


        public async Task InsertarRolAsync()
        {
            if (string.IsNullOrWhiteSpace(NuevoRol.nombre))
                throw new ArgumentException("El nombre del rol es obligatorio.");

            // Verificar si ya existe un rol con el mismo nombre 
            var existente = await rolDAO.ObtenerRolPorNombreAsync(NuevoRol.nombre);
            if (existente != null)
            {
                throw new InvalidOperationException("Ya existe un rol con este nombre.");
            }

            await rolDAO.InsertarRolAsync(NuevoRol);

            var rolCreado = await rolDAO.ObtenerRolPorNombreAsync(NuevoRol.nombre);

            if (rolCreado == null)
                throw new Exception("No se pudo recuperar el rol después de la inserción.");

            var permisosSeleccionados = PermisosDisponibles
                .Where(p => p.IsAssigned)
                .Select(p => p.codigo)
                .ToList();

            foreach (var permisoCodigo in permisosSeleccionados)
            {
                var rolPermiso = new RolPermiso
                {
                    rolId = rolCreado.id,
                    permisoCodigo = permisoCodigo
                };
                await permisoDAO.InsertarRolPermisoAsync(rolPermiso);
            }
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

        public async Task CargarPermisosAsyncN()
        {
            PermisosDisponibles.Clear();

            // Cargar todos los permisos 
            var todosLosPermisos = await permisoDAO.ObtenerPermisosAsync();

            foreach (var permiso in todosLosPermisos)
            {
                permiso.IsAssigned = false; 
                PermisosDisponibles.Add(permiso);
            }
        }


        public async Task GuardarRolAsync()
        {
            if (RolSeleccionado == null)
                throw new ArgumentException("El rol no puede ser nulo.");

            if (string.IsNullOrWhiteSpace(RolSeleccionado.nombre))
                throw new ArgumentException("El nombre del rol es obligatorio.");

            // Verificar si ya existe un rol con el mismo nombre (controla mayusculas)
            var existente = await rolDAO.ObtenerRolPorNombreAsync(RolSeleccionado.nombre);
            if (existente != null && existente.id != RolSeleccionado.id)
            {
                throw new InvalidOperationException("Ya existe un rol con este nombre.");
            }

            await rolDAO.GuardarRolAsync(RolSeleccionado);

            // Guardar permisos asignados
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
