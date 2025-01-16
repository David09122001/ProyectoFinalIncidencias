using ProjecteFinal.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ProjecteFinal.DAO
{
    public class AdjuntoDAO
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        public async Task AñadirAdjuntoAsync(Adjunto adjunto)
        {
            await GetConnection().InsertAsync(adjunto);
        }

        public async Task<List<Adjunto>> ObtenerAdjuntosPorIncidenciaAsync(int incidenciaId)
        {
            return await GetConnection().Table<Adjunto>().Where(a => a.IncidenciaId == incidenciaId).ToListAsync();
        }

        public async Task EliminarAdjuntoAsync(Adjunto adjunto)
        {
            await GetConnection().DeleteAsync(adjunto);
        }
    }
}
