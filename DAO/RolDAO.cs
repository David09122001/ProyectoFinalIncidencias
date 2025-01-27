using ProjecteFinal.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjecteFinal.DAO
{
    public class RolDAO
    {
        private static SQLiteAsyncConnection connection;
        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        public async Task<List<Rol>> ObtenerRolesAsync()
        {
            return await GetConnection().Table<Rol>().ToListAsync();
        }

        public async Task GuardarRolAsync(Rol rol)
        {
            await GetConnection().InsertOrReplaceAsync(rol);
        }

        public async Task<Rol> ObtenerRolPorNombreAsync(string nombre)
        {
            return await GetConnection().Table<Rol>().FirstOrDefaultAsync(r => r.nombre == nombre);
        }

        public async Task<List<Rol>> BuscarTodosAsync()
        {
            return await GetConnection().Table<Rol>().ToListAsync();
        }

        public async Task EliminarRolAsync(Rol rol)
        {
            await GetConnection().DeleteAsync(rol);
        }

        // Asignar permisos a un rol
        public async Task AsignarPermisosAsync(int rolId, List<int> permisosIds)
        {
            // Eliminar los permisos existentes para este rol
            await GetConnection().Table<RolPermiso>().DeleteAsync(rp => rp.rolId == rolId);

            // Insertar los nuevos permisos
            foreach (var permisoId in permisosIds)
            {
                var rolPermiso = new RolPermiso
                {
                    rolId = rolId,
                    permisoCodigo = permisoId
                };
                await GetConnection().InsertAsync(rolPermiso);
            }
        }

        // Eliminar permisos asignados a un rol
        public async Task EliminarPermisosPorRolAsync(int rolId)
        {
            await GetConnection().Table<RolPermiso>().DeleteAsync(rp => rp.rolId == rolId);
        }

        public async Task<Rol> ObtenerRolPorIdAsync(int rolId)
        {
            return await GetConnection().Table<Rol>().FirstOrDefaultAsync(r => r.id == rolId);
        }


    }
}