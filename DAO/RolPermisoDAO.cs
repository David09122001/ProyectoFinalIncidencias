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
    public class RolesPermisosDAO
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        public void AñadirRelacion(RolPermiso relacion)
        {
            GetConnection().InsertAsync(relacion).Wait();
        }

        public ObservableCollection<RolPermiso> ObtenerRelaciones()
        {
            var relacionesQuery = GetConnection().Table<RolPermiso>();
            var relaciones = relacionesQuery.ToListAsync().Result;
            return new ObservableCollection<RolPermiso>(relaciones);
        }

        public void EliminarRelacion(RolPermiso relacion)
        {
            GetConnection().DeleteAsync(relacion).Wait();
        }
    }
}
