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
    public class IncidenciaRedDAO
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        public async Task AñadirIncidenciaRedAsync(Incidencia_Red incidencia)
        {
            await GetConnection().InsertAsync(incidencia);
        }

        public ObservableCollection<Incidencia_Red> ObtenerIncidenciasRed()
        {
            var incidenciasQuery = GetConnection().Table<Incidencia_Red>();
            var incidencias = incidenciasQuery.ToListAsync().Result;
            return new ObservableCollection<Incidencia_Red>(incidencias);
        }

        public void EliminarIncidenciaRed(Incidencia_Red incidencia)
        {
            GetConnection().DeleteAsync(incidencia).Wait();
        }

        public void ActualizarIncidenciaRed(Incidencia_Red incidencia)
        {
            GetConnection().UpdateAsync(incidencia).Wait();
        }

        public async Task<Incidencia_Red> ObtenerIncidenciaRedPorId(int id)
        {
            return await GetConnection().Table<Incidencia_Red>().FirstOrDefaultAsync(i => i.id == id);
        }
    }
}
