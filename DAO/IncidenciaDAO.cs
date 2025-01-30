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
    public class IncidenciaDAO
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        public async Task AñadirIncidenciaAsync(Incidencia incidencia)
        {
            await GetConnection().InsertAsync(incidencia);
        }


        public ObservableCollection<Incidencia> ObtenerIncidencias()
        {
            var incidenciasQuery = GetConnection().Table<Incidencia>();
            var incidencias = incidenciasQuery.ToListAsync().Result;
            return new ObservableCollection<Incidencia>(incidencias);
        }

        public void EliminarIncidencia(Incidencia incidencia)
        {
            GetConnection().DeleteAsync(incidencia).Wait();
        }

        public void ActualizarIncidencia(Incidencia incidencia)
        {
            GetConnection().UpdateAsync(incidencia).Wait();
        }

        public async Task<Incidencia> ObtenerIncidenciaPorId(int id)
        {
            return await GetConnection().Table<Incidencia>().FirstOrDefaultAsync(i => i.id == id);
        }
        public ObservableCollection<Incidencia> ObtenerIncidenciasPorProfesor(string profesorDni)
        {
            var incidenciasQuery = GetConnection()
                .Table<Incidencia>()
                .Where(i => i.profesorDni == profesorDni);

            var incidencias = incidenciasQuery.ToListAsync().Result;
            return new ObservableCollection<Incidencia>(incidencias);
        }


        public ObservableCollection<Incidencia> ObtenerIncidenciasPorEstado(string estado)
        {
            var incidenciasQuery = GetConnection().Table<Incidencia>().Where(i => i.estado == estado);
            var incidencias = incidenciasQuery.ToListAsync().Result;
            return new ObservableCollection<Incidencia>(incidencias);
        }

        public bool EsIncidenciaHardware(int incidenciaId)
        {
            var hw = GetConnection().Table<Incidencia_HW>().FirstOrDefaultAsync(h => h.id == incidenciaId).Result;
            return hw != null;
        }

        public bool EsIncidenciaSoftware(int incidenciaId)
        {
            var sw = GetConnection().Table<Incidencia_SW>().FirstOrDefaultAsync(s => s.id == incidenciaId).Result;
            return sw != null;
        }

        public bool EsIncidenciaRed(int incidenciaId)
        {
            var red = GetConnection().Table<Incidencia_Red>().FirstOrDefaultAsync(r => r.id == incidenciaId).Result;
            return red != null;
        }

        public async Task<Incidencia_HW> ObtenerSubtipoHardwareAsync(int incidenciaId)
        {
            return await GetConnection().Table<Incidencia_HW>().FirstOrDefaultAsync(hw => hw.id == incidenciaId);
        }

        public async Task<Incidencia_SW> ObtenerSubtipoSoftwareAsync(int incidenciaId)
        {
            return await GetConnection().Table<Incidencia_SW>().FirstOrDefaultAsync(sw => sw.id == incidenciaId);
        }

        public async Task<Incidencia_Red> ObtenerSubtipoRedAsync(int incidenciaId)
        {
            return await GetConnection().Table<Incidencia_Red>().FirstOrDefaultAsync(red => red.id == incidenciaId);
        }


        public async Task EliminarSubtipoHardwareAsync(int id)
        {
            var subtipo = await GetConnection().Table<Incidencia_HW>().FirstOrDefaultAsync(hw => hw.id == id);
            if (subtipo != null)
            {
                await GetConnection().DeleteAsync(subtipo);
            }
        }

        public async Task EliminarSubtipoSoftwareAsync(int id)
        {
            var subtipo = await GetConnection().Table<Incidencia_SW>().FirstOrDefaultAsync(sw => sw.id == id);
            if (subtipo != null)
            {
                await GetConnection().DeleteAsync(subtipo);
            }
        }

        public async Task EliminarSubtipoRedAsync(int id)
        {
            var subtipo = await GetConnection().Table<Incidencia_Red>().FirstOrDefaultAsync(red => red.id == id);
            if (subtipo != null)
            {
                await GetConnection().DeleteAsync(subtipo);
            }
        }

        public async Task AñadirSubtipoHardwareAsync(Incidencia_HW incidenciaHW)
        {
            await GetConnection().InsertAsync(incidenciaHW);
        }

        public async Task AñadirSubtipoSoftwareAsync(Incidencia_SW incidenciaSW)
        {
            await GetConnection().InsertAsync(incidenciaSW);
        }

        public async Task AñadirSubtipoRedAsync(Incidencia_Red incidenciaRed)
        {
            await GetConnection().InsertAsync(incidenciaRed);
        }

        private static readonly object _dbLock = new object();

        public async Task ActualizarIncidenciaAsync(Incidencia incidencia)
        {
            lock (_dbLock)
            {
                GetConnection().InsertOrReplaceAsync(incidencia).Wait();
            }
        }


        public async Task AsignarResponsableAsync(int incidenciaId, string profesorDni)
        {
            var incidencia = await ObtenerIncidenciaPorId(incidenciaId);
            if (incidencia != null)
            {
                incidencia.responsableDni = profesorDni;
                await GetConnection().UpdateAsync(incidencia);
            }
        }

        public List<int> ObtenerIdsDeIncidenciasHardware()
        {
            var incidenciasHW = GetConnection().Table<Incidencia_HW>().ToListAsync().Result;
            return incidenciasHW.Select(hw => hw.id).ToList();
        }

        public List<int> ObtenerIdsDeIncidenciasSoftware()
        {
            var incidenciasSW = GetConnection().Table<Incidencia_SW>().ToListAsync().Result;
            return incidenciasSW.Select(sw => sw.id).ToList(); 
        }

        public List<int> ObtenerIdsDeIncidenciasRed()
        {
            var incidenciasRed = GetConnection().Table<Incidencia_Red>().ToListAsync().Result;
            return incidenciasRed.Select(red => red.id).ToList(); 
        }

        public async Task<List<Incidencia>> FiltrarIncidenciasAsync(string estado = null, string profesorDni = null, string tipoIncidencia = null)
        {
            var incidencias = await GetConnection().Table<Incidencia>().ToListAsync();

            // Filtrar por estado
            if (!string.IsNullOrEmpty(estado))
            {
                incidencias = incidencias.Where(i => i.estado == estado).ToList();
            }

            // Filtrar por profesor
            if (!string.IsNullOrEmpty(profesorDni))
            {
                incidencias = incidencias.Where(i => i.profesorDni == profesorDni).ToList();
            }

            // Filtrar por tipo de incidencia
            if (!string.IsNullOrEmpty(tipoIncidencia))
            {
                List<int> ids = tipoIncidencia switch
                {
                    "Hardware" => (await GetConnection().Table<Incidencia_HW>().ToListAsync()).Select(hw => hw.id).ToList(),
                    "Software" => (await GetConnection().Table<Incidencia_SW>().ToListAsync()).Select(sw => sw.id).ToList(),
                    "Red" => (await GetConnection().Table<Incidencia_Red>().ToListAsync()).Select(red => red.id).ToList(),
                    _ => new List<int>()
                };

                incidencias = incidencias.Where(i => ids.Contains(i.id)).ToList();
            }

            return incidencias;
        }

        public async Task<Dictionary<string, int>> ObtenerEstadisticasPorTipoAsync()
        {
            var hardwareCount = (await GetConnection().Table<Incidencia_HW>().ToListAsync()).Count;
            var softwareCount = (await GetConnection().Table<Incidencia_SW>().ToListAsync()).Count;
            var redCount = (await GetConnection().Table<Incidencia_Red>().ToListAsync()).Count;

            return new Dictionary<string, int>
    {
        { "Hardware", hardwareCount },
        { "Software", softwareCount },
        { "Red", redCount }
    };
        }


        public async Task<Dictionary<string, int>> ObtenerEstadisticasPorEstadoAsync()
        {
            var incidencias = await GetConnection().Table<Incidencia>().ToListAsync();

            var estadisticas = incidencias
                .GroupBy(i => i.estado)
                .ToDictionary(g => g.Key, g => g.Count());

            return estadisticas;
        }


        public async Task<int> ObtenerTotalIncidenciasAsync()
        {
            return await GetConnection().Table<Incidencia>().CountAsync();
        }





    }
}