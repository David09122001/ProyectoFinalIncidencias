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


    }
}
