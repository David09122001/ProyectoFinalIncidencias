﻿using ProjecteFinal.Models;
using SQLite;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ProjecteFinal.DAO
{
    public class LogDAO
    {
        private static SQLiteAsyncConnection connection;

        public LogDAO()
        {
            connection = BaseDatos.BaseDatos.GetConnection();
        }

        // Método para añadir un log de forma asincrónica
        public async Task AñadirLogAsync(Log log)
        {
            await connection.InsertAsync(log);
            Console.WriteLine($"Log añadido: IncidenciaId = {log.incidenciaId}, Estado = {log.estado}, Fecha = {log.fecha}");
        }

        // Método para obtener todos los logs
        public async Task<ObservableCollection<Log>> ObtenerLogsAsync()
        {
            var logs = await connection.Table<Log>().ToListAsync();
            return new ObservableCollection<Log>(logs);
        }

        // Método para obtener los logs específicos de una incidencia
        public async Task<ObservableCollection<Log>> ObtenerLogsPorIncidenciaAsync(int incidenciaId)
        {
            var logs = await connection.Table<Log>()
                                        .Where(l => l.incidenciaId == incidenciaId)
                                        .ToListAsync();
            return new ObservableCollection<Log>(logs);
        }

        // Método para eliminar un log
        public async Task EliminarLogAsync(Log log)
        {
            await connection.DeleteAsync(log);
            Console.WriteLine($"Log eliminado: Id = {log.id}");
        }

        // Método para actualizar un log existente
        public async Task ActualizarLogAsync(Log log)
        {
            await connection.UpdateAsync(log);
            Console.WriteLine($"Log actualizado: Id = {log.id}");
        }
    }
}
