using ProjecteFinal.Models;
using SQLite;
using System.Threading.Tasks;

namespace ProjecteFinal.DAO
{
    public class ProfesorDAO
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        public async Task ActualizarProfesorAsync(Profesor profesor)
        {
            await GetConnection().UpdateAsync(profesor);
        }

        public async Task<Profesor> ObtenerProfesorPorCorreoAsync(string correo)
        {
            return await GetConnection().Table<Profesor>().FirstOrDefaultAsync(p => p.email == correo);
        }

        public async Task<List<Profesor>> BuscarTodosAsync()
        {
            return await GetConnection().Table<Profesor>().ToListAsync();
        }

        public void EliminarProfesor(Profesor profesor)
        {
            GetConnection().DeleteAsync(profesor).Wait();
        }
        public async Task<Profesor> BuscarPorDniAsync(string dni)
        {
            return await GetConnection().Table<Profesor>().FirstOrDefaultAsync(p => p.dni == dni);
        }
        public async Task<List<Profesor>> BuscarPorDepartamento(string departamentoCodigo)
        {
            return await GetConnection()
                .Table<Profesor>()
                .Where(p => p.departamentoCodigo == departamentoCodigo)
                .ToListAsync();
        }

        public async Task InsertarProfesorAsync(Profesor profesor)
        {
            await GetConnection().InsertOrReplaceAsync(profesor);
        }



    }
}
