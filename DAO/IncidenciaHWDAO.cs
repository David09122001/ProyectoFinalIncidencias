using ProjecteFinal.Models;
using SQLite;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace ProjecteFinal.DAO
{
    public class IncidenciaHWDAO
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        // Añadir un nuevo registro (puede ser tipo o incidencia)
        public async Task AñadirIncidenciaHWAsync(Incidencia_HW incidencia)
        {
            await GetConnection().InsertAsync(incidencia);
        }

        // Obtener todas las incidencias de HW
        public async Task<ObservableCollection<Incidencia_HW>> ObtenerIncidenciasHWAsync()
        {
            var incidencias = await GetConnection().Table<Incidencia_HW>().Where(i => i.id > 0).ToListAsync();
            return new ObservableCollection<Incidencia_HW>(incidencias);
        }

        // Obtener solo los tipos de HW
        public async Task<ObservableCollection<Incidencia_HW>> ObtenerTiposHWAsync()
        {
            var tipos = await GetConnection().Table<Incidencia_HW>().Where(i => i.id < 0).ToListAsync();
            return new ObservableCollection<Incidencia_HW>(tipos);
        }

        // Eliminar un registro (tipo o incidencia)
        public async Task EliminarIncidenciaHWAsync(Incidencia_HW incidencia)
        {
            await GetConnection().DeleteAsync(incidencia);
        }

        // Actualizar un registro existente
        public async Task ActualizarIncidenciaHWAsync(Incidencia_HW tipo, string nuevoNombre)
        {
            // Validar duplicado (excluir el tipo actual por su ID)
            var duplicado = await GetConnection()
                .Table<Incidencia_HW>()
                .FirstOrDefaultAsync(hw => hw.dispositivo.ToLower() == nuevoNombre.ToLower() && hw.id != tipo.id && hw.id < 0);

            if (duplicado != null)
            {
                throw new InvalidOperationException("Ya existe un tipo de hardware con este nombre.");
            }

            // Actualizar el tipo con el nuevo nombre
            tipo.dispositivo = nuevoNombre;
            await GetConnection().UpdateAsync(tipo);
        }


        // Obtener un registro específico por ID
        public async Task<Incidencia_HW> ObtenerIncidenciaHWPorIdAsync(int id)
        {
            return await GetConnection().Table<Incidencia_HW>().FirstOrDefaultAsync(i => i.id == id);
        }

        // Añadir un tipo de HW con ID negativo
        public async Task AñadirTipoHWAsync(string nombre)
        {
            // Validar duplicado
            var duplicado = await GetConnection()
                .Table<Incidencia_HW>()
                .FirstOrDefaultAsync(hw => hw.dispositivo.ToLower() == nombre.ToLower() && hw.id < 0);

            if (duplicado != null)
            {
                throw new InvalidOperationException("El tipo de hardware ya existe.");
            }

            // Obtener el siguiente ID negativo disponible
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
