using ProjecteFinal.Models;
using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public async Task EliminarAdjuntoAsync(Adjunto adjunto)
        {
            await GetConnection().DeleteAsync(adjunto);
        }

        public async Task<ObservableCollection<Adjunto>> ObtenerAdjuntosPorIncidenciaAsync(int incidenciaId)
        {
            var adjuntos = await GetConnection()
                .Table<Adjunto>()
                .Where(a => a.IncidenciaId == incidenciaId)
                .ToListAsync();

            return new ObservableCollection<Adjunto>(adjuntos);
        }

        // Implementación del método ActualizarAdjuntoAsync
        public async Task ActualizarAdjuntoAsync(Adjunto adjunto)
        {
            await GetConnection().UpdateAsync(adjunto);
        }
    }
}
