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

        public async Task InsertarRolAsync(Rol rol)
        {
            await GetConnection().InsertAsync(rol);
        }

        public async Task<Rol> ObtenerRolPorNombreAsync(string nombre)
        {
            return await GetConnection()
                .Table<Rol>()
                .FirstOrDefaultAsync(r => r.nombre.ToLower() == nombre.ToLower());
        }

        public async Task EliminarRolAsync(Rol rol)
        {
            await GetConnection().DeleteAsync(rol);
        }

        public async Task<Rol> ObtenerRolPorIdAsync(int rolId)
        {
            return await GetConnection().Table<Rol>().FirstOrDefaultAsync(r => r.id == rolId);
        }


    }
}