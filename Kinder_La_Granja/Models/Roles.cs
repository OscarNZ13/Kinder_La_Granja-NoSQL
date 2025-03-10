using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kinder_La_Granja.Models;

public class Roles
{
    [BsonId]  // Indica que es el ID del documento
    [BsonRepresentation(BsonType.ObjectId)] // Indicando que usamos el ObjectId
    public string Id { get; set; }

    [BsonElement("nombre")]
    public string Nombre { get; set; }
}