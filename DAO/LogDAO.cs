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
    public class LogDAO
    {
        private static SQLiteAsyncConnection connection;

        public static SQLiteAsyncConnection GetConnection()
        {
            return BaseDatos.BaseDatos.GetConnection();
        }

        public void AñadirLog(Log log)
        {
            GetConnection().InsertAsync(log).Wait();
        }

        public ObservableCollection<Log> ObtenerLogs()
        {
            var logsQuery = GetConnection().Table<Log>();
            var logs = logsQuery.ToListAsync().Result;
            return new ObservableCollection<Log>(logs);
        }

        public void EliminarLog(Log log)
        {
            GetConnection().DeleteAsync(log).Wait();
        }

        public void ActualizarLog(Log log)
        {
            GetConnection().UpdateAsync(log).Wait();
        }
    }
}
