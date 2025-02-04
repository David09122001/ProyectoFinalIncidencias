using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjecteFinal.Models
{
    public class Log
    {
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        //[ForeignKey(typeof(Incidencia)), NotNull]
        public int incidenciaId { get; set; }

        [NotNull]
        public string estado { get; set; }

        public DateTime fecha { get; set; } = DateTime.Now;
    }
}
