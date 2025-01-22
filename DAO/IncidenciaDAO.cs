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

        // Eliminar una incidencia
        public void EliminarIncidencia(Incidencia incidencia)
        {
            GetConnection().DeleteAsync(incidencia).Wait();
        }

        // Actualizar una incidencia
        public void ActualizarIncidencia(Incidencia incidencia)
        {
            GetConnection().UpdateAsync(incidencia).Wait();
        }

        // Obtener incidencia por ID
        public async Task<Incidencia> ObtenerIncidenciaPorId(int id)
        {
            return await GetConnection().Table<Incidencia>().FirstOrDefaultAsync(i => i.id == id);
        }

        // Obtener incidencias por profesor
        public ObservableCollection<Incidencia> ObtenerIncidenciasPorProfesor(string profesorDni)
        {
            var incidenciasQuery = GetConnection().Table<Incidencia>().Where(i => i.profesorDni == profesorDni);
            var incidencias = incidenciasQuery.ToListAsync().Result;
            return new ObservableCollection<Incidencia>(incidencias);
        }

        // Obtener incidencias por estado
        public ObservableCollection<Incidencia> ObtenerIncidenciasPorEstado(string estado)
        {
            var incidenciasQuery = GetConnection().Table<Incidencia>().Where(i => i.estado == estado);
            var incidencias = incidenciasQuery.ToListAsync().Result;
            return new ObservableCollection<Incidencia>(incidencias);
        }

        // Verificar si una incidencia es de tipo Hardware
        public bool EsIncidenciaHardware(int incidenciaId)
        {
            var hw = GetConnection().Table<Incidencia_HW>().FirstOrDefaultAsync(h => h.id == incidenciaId).Result;
            return hw != null;
        }

        // Verificar si una incidencia es de tipo Software
        public bool EsIncidenciaSoftware(int incidenciaId)
        {
            var sw = GetConnection().Table<Incidencia_SW>().FirstOrDefaultAsync(s => s.id == incidenciaId).Result;
            return sw != null;
        }

        // Verificar si una incidencia es de tipo Red
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


    }
}