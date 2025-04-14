using Kinder_La_Granja.Interface;
using Kinder_La_Granja.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kinder_La_Granja.Data;

public class NinosDBContext : INinos
{
    private readonly IMongoCollection<Ninos> _ninos;

    public NinosDBContext(IOptions<MongoDBSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.Database);
        _ninos = database.GetCollection<Ninos>("Ninos");
    }

    public async Task<List<Ninos>> GetAllAsync()
    {
        return await _ninos.Find(_ => true).ToListAsync();
    }

    public async Task<List<Ninos>> GetByNivelAsync(int nivelId)
    {
        return await _ninos.Find(n => n.nivel == nivelId).ToListAsync();
    }

    public async Task<Ninos> GetByIdAsync(ObjectId id)
    {
        return await _ninos.Find(n => n._id == id).FirstOrDefaultAsync();
    }

    public async Task CreateAsync(Ninos nino)
    {
        await _ninos.InsertOneAsync(nino);
    }

    public async Task UpdateAsync(ObjectId id, Ninos nino)
    {
        await _ninos.ReplaceOneAsync(n => n._id == id, nino);
    }

    public async Task DeleteAsync(ObjectId id)
    {
        await _ninos.DeleteOneAsync(n => n._id == id);
    }



}