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

        private static readonly SemaphoreSlim _dbSemaphore = new SemaphoreSlim(1, 1);

        public async Task ActualizarIncidenciaAsync(Incidencia incidencia)
        {
            await _dbSemaphore.WaitAsync(); // Bloquea otros accesos a la BD
            try
            {
                await GetConnection().InsertOrReplaceAsync(incidencia);
            }
            finally
            {
                _dbSemaphore.Release(); // Libera el acceso
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

     

    }
}