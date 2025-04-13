using Kinder_La_Granja.Interface;
using Kinder_La_Granja.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kinder_La_Granja.Data;

public class Condiciones_MedicasDBContext : ICondiciones_Medicas
{
    private readonly IMongoDatabase _db;
    //private readonly IMongoCollection<Condiciones_Medicas> _Condiciones_Medicas;

    public Condiciones_MedicasDBContext(IOptions<MongoDBSettings> options)
    {
        var client = new MongoClient(options.Value.ConnectionString);
        _db = client.GetDatabase(options.Value.Database);
        //var database = client.GetDatabase(options.Value.Database);
        Condiciones_MedicasCollection = _db.GetCollection<Condiciones_Medicas>("Condiciones_Medicas");
        //_Condiciones_Medicas = database.GetCollection<Condiciones_Medicas>("Condiciones_Medicas");
    }

    public IMongoCollection<Condiciones_Medicas> Condiciones_MedicasCollection { get; }

    public IMongoCollection<Condiciones_Medicas> Condiciones_Medicas { get; }

    public IEnumerable<Condiciones_Medicas> GetAllCondiciones_Medicas()
    {
        return Condiciones_MedicasCollection.Find(Condiciones_Medicas => true).ToList();
    }

    public void CreateCondiciones_Medicas(Condiciones_Medicas Condiciones_Medicas)
    {
        Condiciones_MedicasCollection.InsertOne(Condiciones_Medicas);
    }


    public Condiciones_Medicas GetCondiciones_MedicasById(string id)
    {
        var objectId = ObjectId.Parse(id); // convertimos el id string a ObjectId, sino no reconocera el id ya que es string
        return Condiciones_MedicasCollection.Find(u => u.Id == objectId).FirstOrDefault();
    }


    public void UpdateCondiciones_Medicas(string id, Condiciones_Medicas Condiciones_Medicas)
    {
        var objectId = ObjectId.Parse(id);
        var filter = Builders<Condiciones_Medicas>.Filter.Eq(u => u.Id, objectId);

        var update = Builders<Condiciones_Medicas>.Update
            .Set(u => u.nombre, Condiciones_Medicas.nombre);
           

        var result = Condiciones_MedicasCollection.UpdateOne(filter, update);
    }

    public void DeleteCondiciones_Medicas(string id)
    {
        var objectId = ObjectId.Parse(id);
        var filter = Builders<Condiciones_Medicas>.Filter.Eq(u => u.Id, objectId);
        Condiciones_MedicasCollection.DeleteOne(filter);
    }


   
}