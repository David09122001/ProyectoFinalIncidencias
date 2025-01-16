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
            var incidenciasQuery = GetConnection().Table<Incidencia>().Where(i => i.profesorDni == profesorDni);
            var incidencias = incidenciasQuery.ToListAsync().Result;
            return new ObservableCollection<Incidencia>(incidencias);
        }

        public ObservableCollection<Incidencia> ObtenerIncidenciasPorEstado(string estado)
        {
            var incidenciasQuery = GetConnection().Table<Incidencia>().Where(i => i.estado == estado);
            var incidencias = incidenciasQuery.ToListAsync().Result;
            return new ObservableCollection<Incidencia>(incidencias);
        }
    }
}
