using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjecteFinal.Models
{
    public class Incidencia_Red
    {
        [PrimaryKey, ForeignKey(typeof(Incidencia))]
        public int id { get; set; }

        [MaxLength(50)]
        public string dispositivoAfectado { get; set; }
    }
}
