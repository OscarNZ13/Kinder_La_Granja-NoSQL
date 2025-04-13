using Microsoft.AspNetCore.Mvc.Rendering;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography.Xml;

namespace Kinder_La_Granja.Models
{
    public class Tareas
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId _id { get; set; }

        public int id_tarea { get; set; }

        [BsonRepresentation(BsonType.ObjectId)]
        [Required(ErrorMessage = "Debe seleccionar un docente")]
        public ObjectId id_usuarioDocente { get; set; }

        [Required(ErrorMessage = "El nombre de la tarea es obligatorio")]
        public string nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria")]
        public string descripcion { get; set; }

        [Required(ErrorMessage = "Debe asignar una fecha")]
        [DataType(DataType.Date)]
        public DateTime fecha_asignada { get; set; }

        [Required(ErrorMessage = "Debe indicar una fecha de entrega")]
        [DataType(DataType.Date)]
        public DateTime fecha_entrega { get; set; }
    }
}

