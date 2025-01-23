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

        public void AñadirRol(Rol rol)
        {
            GetConnection().InsertAsync(rol).Wait();
        }

        public async Task<ObservableCollection<Rol>> ObtenerRolesAsync()
        {
            try
            {
                var roles = await GetConnection().Table<Rol>().ToListAsync();
                return new ObservableCollection<Rol>(roles);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al obtener roles: {ex.Message}");
                return new ObservableCollection<Rol>(); 
            }
        }


        public void EliminarRol(Rol rol)
        {
            GetConnection().DeleteAsync(rol).Wait();
        }

        public void ActualizarRol(Rol rol)
        {
            GetConnection().UpdateAsync(rol).Wait();
        }
    }
}
