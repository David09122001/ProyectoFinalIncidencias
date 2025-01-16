using SQLite;
using SQLiteNetExtensions.Attributes;

namespace ProjecteFinal.Models
{
    public class Adjunto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        [ForeignKey(typeof(Incidencia))]
        public int IncidenciaId { get; set; }

        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public byte[] Datos { get; set; }
    }
}