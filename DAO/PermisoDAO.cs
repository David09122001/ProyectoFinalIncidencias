using GestorIncidencias.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorIncidencias.DAO
{
    public class PermisoDAO
    {
        private SQLiteAsyncConnection connection;

        public PermisoDAO()
        {
            connection = BaseDatos.BaseDatos.GetConnection();
        }

        public async Task EliminarPermisosPorRolAsync(int rolId)
        {
            await connection.Table<RolPermiso>()
                            .Where(rp => rp.rolId == rolId)
                            .DeleteAsync();
        }

        public async Task InsertarRolPermisoAsync(RolPermiso rolPermiso)
        {
            await connection.InsertAsync(rolPermiso);
        }

        public async Task<List<Permiso>> ObtenerPermisosAsync()
        {
            return await connection.Table<Permiso>().ToListAsync();
        }

        public async Task<List<Permiso>> ObtenerPermisosPorRolAsync(int rolId)
        {
            var rolPermisos = await connection.Table<RolPermiso>()
                                                .Where(rp => rp.rolId == rolId)
                                                .ToListAsync();
            var permisoIds = rolPermisos.Select(rp => rp.permisoCodigo).ToList();
            return await connection.Table<Permiso>()
                                   .Where(p => permisoIds.Contains(p.codigo))
                                   .ToListAsync();
        }
    }
}
