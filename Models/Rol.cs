using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjecteFinal.Models
{
    public class Rol
    {
    
        [PrimaryKey, AutoIncrement]
        public int id { get; set; }

        [MaxLength(50), NotNull]
        public string nombre { get; set; }
    
}
}
