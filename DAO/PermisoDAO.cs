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
    public class PermisoDAO
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        public void AñadirPermiso(Permiso permiso)
        {
            GetConnection().InsertAsync(permiso).Wait();
        }

        public ObservableCollection<Permiso> ObtenerPermisos()
        {
            var permisosQuery = GetConnection().Table<Permiso>();
            var permisos = permisosQuery.ToListAsync().Result;
            return new ObservableCollection<Permiso>(permisos);
        }

        public void EliminarPermiso(Permiso permiso)
        {
            GetConnection().DeleteAsync(permiso).Wait();
        }

        public void ActualizarPermiso(Permiso permiso)
        {
            GetConnection().UpdateAsync(permiso).Wait();
        }
    }
}
