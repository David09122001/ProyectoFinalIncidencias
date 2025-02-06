using SQLite;
using SQLiteNetExtensions.Attributes;

namespace GestorIncidencias.Models
{
    public class Adjunto
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string Nombre { get; set; }
        public string Ruta { get; set; }
        public byte[] Datos { get; set; }
        public int IncidenciaId { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is Adjunto otroAdjunto)
            {
                return Id == otroAdjunto.Id;
            }
            return false;
        }

        public override int GetHashCode() => Id.GetHashCode();
    }

}