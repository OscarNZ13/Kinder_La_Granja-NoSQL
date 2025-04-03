using Kinder_La_Granja.Interface;
using Kinder_La_Granja.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kinder_La_Granja.Data;

public class NivelDBContext : INivel
{
    private readonly IMongoCollection<Nivel> _niveles;

    public NivelDBContext(IOptions<MongoDBSettings> settings)
    {
        var client = new MongoClient(settings.Value.ConnectionString);
        var database = client.GetDatabase(settings.Value.Database);
        _niveles = database.GetCollection<Nivel>("Niveles");
    }

    public async Task<List<Nivel>> GetAllAsync()
    {
        return await _niveles.Find(_ => true).ToListAsync();
    }
}