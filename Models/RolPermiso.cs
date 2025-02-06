using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestorIncidencias.Models
{
    public class RolPermiso
    {
        [ForeignKey(typeof(Rol)), NotNull]
        public int rolId { get; set; }

        [ForeignKey(typeof(Permiso)), NotNull]
        public int permisoCodigo { get; set; }

    }
}
