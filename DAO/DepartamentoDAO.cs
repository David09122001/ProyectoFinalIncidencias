using GestorIncidencias.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GestorIncidencias.DAO
{
    public class DepartamentoDAO
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        public async Task<List<Departamento>> ObtenerDepartamentosAsync()
        {
            return await GetConnection().Table<Departamento>().ToListAsync();
        }

        public async Task ActualizarDepartamentoAsync(Departamento departamento)
        {
            await GetConnection().UpdateAsync(departamento);
        }

        public async Task GuardarDepartamentoAsync(Departamento departamento)
        {
            await GetConnection().InsertAsync(departamento);
        }

        public async Task<Departamento> ObtenerDepartamentoPorCodigoAsync(string codigo)
        {
            return await GetConnection().Table<Departamento>().FirstOrDefaultAsync(d => d.codigo == codigo);
        }

        public async Task<List<Departamento>> BuscarTodosAsync()
        {
            return await GetConnection().Table<Departamento>().ToListAsync();
        }
        public async Task EliminarDepartamentoAsync(Departamento departamento)
        {
            await GetConnection().DeleteAsync(departamento);
        }

    }
}
