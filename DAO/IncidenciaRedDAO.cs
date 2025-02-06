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
    public class IncidenciaRedDAO
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        public async Task<Incidencia_Red> ObtenerIncidenciaRedPorId(int id)
        {
            return await GetConnection().Table<Incidencia_Red>().FirstOrDefaultAsync(i => i.id == id);
        }
    }
}
