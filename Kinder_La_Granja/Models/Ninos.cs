using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Xml;

namespace Kinder_La_Granja.Models
{
    public class Referencia
    {
        public string Parentesco { get; set; }
        public string Nombre { get; set; }
        public int Cedula { get; set; }
        public int Telefono { get; set; }
    }

    public class Ninos
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        public int id { get; set; }

        [Required]
        public int cedula { get; set; }

        [Required]
        public string nombre { get; set; }

        [DataType(DataType.Date)]
        public DateTime fecha_nacimiento { get; set; }

        public string direccion { get; set; }
        public string poliza { get; set; }

        [Required]
        public int nivel { get; set; }

        public List<int>? condiciones_medicas { get; set; }

        public List<Referencia> Referencias { get; set; }
    }

}