using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjecteFinal.Models
{
    public class Incidencia_SW
    {
        [PrimaryKey, ForeignKey(typeof(Incidencia))]
        public int id { get; set; }

        public string sistemaOperativo { get; set; }
        public string aplicacion { get; set; }
    }
}
