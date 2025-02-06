using GestorIncidencias.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorIncidencias.DAO
{
    public class IncidenciaSWDAO
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        public async Task AñadirIncidenciaSWAsync(Incidencia_SW incidencia)
        {
            await GetConnection().InsertAsync(incidencia);
        }
        public async Task<Incidencia_SW> ObtenerIncidenciaSWPorId(int id)
        {
            return await GetConnection().Table<Incidencia_SW>().FirstOrDefaultAsync(i => i.id == id);
        }
    }
}
