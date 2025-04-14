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

    public async Task<List<Tareas>> GetTareasByNinoIdAsync(ObjectId ninoId)
    {
        var nino = await _ninos.Find(n => n._id == ninoId).FirstOrDefaultAsync();

        if (nino != null && nino.tareas != null && nino.tareas.Any())
        {
            return nino.tareas.Select(tareaId => new Tareas
            {
                _id = tareaId,
            }).ToList();
        }

        return new List<Tareas>();
    }

    public async Task UpdateAsync(string id, Ninos nino)
    {
        var objectId = ObjectId.Parse(id);

        // Aquí actualizas el niño en la base de datos
        var result = await _ninos.ReplaceOneAsync(
            filter: n => n._id == objectId,
            replacement: nino
        );

        if (result.MatchedCount == 0)
        {
            throw new Exception("No se pudo actualizar el niño. Puede que no exista.");
        }
    }
}