using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.ComponentModel.DataAnnotations;

namespace Kinder_La_Granja.Models
{
    public class Usuarios
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public ObjectId Id { get; set; }

        [BsonElement("cedula")]
        [Required(ErrorMessage = "La cédula es obligatoria.")]
        public int Cedula { get; set; }

        [BsonElement("nombre")]
        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        public string Nombre { get; set; }

        [BsonElement("contrasena_hash")]
        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [RegularExpression("^(?=.*[a-z])(?=.*[A-Z])(?=.*\\d).+$",
            ErrorMessage = "La contraseña debe contener al menos una letra mayúscula, una minúscula y un número.")]
        public string ContrasenaHash { get; set; }

        [BsonElement("num_telefono")] public int NumTelefono { get; set; }

        [BsonElement("direccion")]
        [MaxLength(120, ErrorMessage = "La dirección no puede exceder los 120 caracteres.")]
        public string Direccion { get; set; }

        [BsonElement("correo_electronico")]
        [EmailAddress(ErrorMessage = "Por favor ingresa un correo electrónico válido.")]
        [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
        public string CorreoElectronico { get; set; }

        [BsonElement("id_rol")] public int IdRol { get; set; }
    }
}