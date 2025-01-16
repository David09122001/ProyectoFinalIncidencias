using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjecteFinal.Models
{
    public class Profesor
    {
        [PrimaryKey, MaxLength(9)]
        public string dni { get; set; }

        [MaxLength(100), NotNull]
        public string nombre { get; set; }

        [ForeignKey(typeof(Departamento)), NotNull]
        public string departamentoCodigo { get; set; }

        [MaxLength(100), NotNull, Unique]
        public string email { get; set; }

        [MaxLength(100), NotNull]
        public string contrasena { get; set; }

        [ForeignKey(typeof(Rol)), NotNull]
        public int rol_id { get; set; }
    }
}
