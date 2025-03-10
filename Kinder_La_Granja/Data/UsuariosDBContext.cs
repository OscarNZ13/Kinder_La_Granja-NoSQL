using Kinder_La_Granja.Interface;
using Kinder_La_Granja.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kinder_La_Granja.Data;

public class UsuariosDBContext : IUsuarios
{
    private readonly IMongoDatabase _db;

    public UsuariosDBContext(IOptions<MongoDBSettings> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        _db = client.GetDatabase(options.Value.Database);
        UsuariosCollection = _db.GetCollection<Usuarios>("Usuarios");
    }

    public IMongoCollection<Usuarios> UsuariosCollection { get; }

    public IMongoCollection<Usuarios> Usuarios { get; }

    public IEnumerable<Usuarios> GetAllUsuarios()
    {
        return UsuariosCollection.Find(usuario => true).ToList();
    }

    public Usuarios GetUsuarioById(string id)
    {
        var objectId = ObjectId.Parse(id); // convertimos el id string a ObjectId, sino no reconocera el id ya que es string
        return UsuariosCollection.Find(u => u.Id == objectId).FirstOrDefault();
    }

    public Usuarios GetUsuarioByEmail(string email)
    {
        return UsuariosCollection.Find(u => u.CorreoElectronico == email).FirstOrDefault();
    }

    public void CreateUsuario(Usuarios usuario)
    {
        UsuariosCollection.InsertOne(usuario);
    }

    public void UpdateUsuario(string id, Usuarios usuario)
    {
        var objectId = ObjectId.Parse(id);
        usuario.Id = objectId;
        var filter = Builders<Usuarios>.Filter.Eq(u => u.Id, objectId);
        UsuariosCollection.ReplaceOne(filter, usuario);
    }

    public void DeleteUsuario(string id)
    {
        var objectId = ObjectId.Parse(id);
        var filter = Builders<Usuarios>.Filter.Eq(u => u.Id, objectId);
        UsuariosCollection.DeleteOne(filter);
    }
}