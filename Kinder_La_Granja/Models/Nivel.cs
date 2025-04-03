using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Kinder_La_Granja.Models
{
    public class Nivel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        public int id { get; set; } 

        [Required(ErrorMessage = "El nombre del nivel es obligatorio")]
        public string nombre { get; set; }


        public int edad_min { get; set; }
        public int edad_max { get; set; }

        
    }
}