using GestorIncidencias.Models;
using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace GestorIncidencias.DAO
{
    public class IncidenciaHWDAO
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        public async Task AñadirIncidenciaHWAsync(Incidencia_HW incidencia)
        {
            await GetConnection().InsertAsync(incidencia);
        }

        public async Task<ObservableCollection<Incidencia_HW>> ObtenerTiposHWAsync()
        {
            var tipos = await GetConnection().Table<Incidencia_HW>().Where(i => i.id < 0).ToListAsync();
            return new ObservableCollection<Incidencia_HW>(tipos);
        }

        public async Task EliminarIncidenciaHWAsync(Incidencia_HW incidencia)
        {
            await GetConnection().DeleteAsync(incidencia);
        }

        public async Task ActualizarIncidenciaHWAsync(Incidencia_HW tipo, string nuevoNombre)
        {
            var duplicado = await GetConnection()
                .Table<Incidencia_HW>()
                .FirstOrDefaultAsync(hw => hw.dispositivo.ToLower() == nuevoNombre.ToLower() && hw.id != tipo.id && hw.id < 0);

            if (duplicado != null)
            {
                throw new InvalidOperationException("Ya existe un tipo de hardware con este nombre.");
            }

            tipo.dispositivo = nuevoNombre;
            await GetConnection().UpdateAsync(tipo);
        }


        public async Task<Incidencia_HW> ObtenerIncidenciaHWPorIdAsync(int id)
        {
            return await GetConnection().Table<Incidencia_HW>().FirstOrDefaultAsync(i => i.id == id);
        }

        public async Task AñadirTipoHWAsync(string nombre)
        {
            var duplicado = await GetConnection()
                .Table<Incidencia_HW>()
                .FirstOrDefaultAsync(hw => hw.dispositivo.ToLower() == nombre.ToLower() && hw.id < 0);

            if (duplicado != null)
            {
                throw new InvalidOperationException("El tipo de hardware ya existe.");
            }

            var tiposExistentes = await GetConnection()
                .Table<Incidencia_HW>()
                .Where(hw => hw.id < 0)
                .ToListAsync();

            int nuevoId = tiposExistentes.Any() ? tiposExistentes.Min(hw => hw.id) - 1 : -1;

            var tipoHW = new Incidencia_HW
            {
                id = nuevoId,
                dispositivo = nombre
            };

            await GetConnection().InsertAsync(tipoHW);
        }

    }
}
