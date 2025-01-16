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

        public ObservableCollection<Incidencia_SW> ObtenerIncidenciasSW()
        {
            var incidenciasQuery = GetConnection().Table<Incidencia_SW>();
            var incidencias = incidenciasQuery.ToListAsync().Result;
            return new ObservableCollection<Incidencia_SW>(incidencias);
        }

        public void EliminarIncidenciaSW(Incidencia_SW incidencia)
        {
            GetConnection().DeleteAsync(incidencia).Wait();
        }

        public void ActualizarIncidenciaSW(Incidencia_SW incidencia)
        {
            GetConnection().UpdateAsync(incidencia).Wait();
        }

        public async Task<Incidencia_SW> ObtenerIncidenciaSWPorId(int id)
        {
            return await GetConnection().Table<Incidencia_SW>().FirstOrDefaultAsync(i => i.id == id);
        }
    }
}
