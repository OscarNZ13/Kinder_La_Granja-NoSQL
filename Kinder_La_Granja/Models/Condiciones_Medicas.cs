using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Kinder_La_Granja.Models
{
    public class Condiciones_Medicas
    {
        [BsonId] // Representa el campo '_id' en MongoDB.
        [BsonRepresentation(BsonType.ObjectId)] // Convierte automáticamente a string para trabajar con ObjectId.
        public ObjectId Id { get; set; }

       
        [BsonElement("id")] // Representa el campo 'id' en MongoDB.
        public int Ids { get; set; }

        [BsonElement("nombre")] // Representa el campo 'nombre' en MongoDB.
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        public string nombre { get; set; }



    }
}