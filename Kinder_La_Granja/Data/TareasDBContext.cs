﻿using Kinder_La_Granja.Interface;
using Kinder_La_Granja.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kinder_La_Granja.Data
{
    public class TareasDBContext : ITareas
    {
        private readonly IMongoCollection<Tareas> _tareas;

        public TareasDBContext(IOptions<MongoDBSettings> settings, IMongoClient client)
        {
            var database = client.GetDatabase(settings.Value.Database);
            _tareas = database.GetCollection<Tareas>("Tareas");
        }

        public async Task<List<Tareas>> GetAllAsync() =>
            await _tareas.Find(_ => true).ToListAsync();

        public async Task<Tareas> GetByIdAsync(string id) =>
            await _tareas.Find(t => t._id == ObjectId.Parse(id)).FirstOrDefaultAsync();

        public async Task<List<Tareas>> GetByDocenteAsync(string docenteId) =>
            await _tareas.Find(t => t.id_usuarioDocente == ObjectId.Parse(docenteId)).ToListAsync();

        public async Task CreateAsync(Tareas t) =>
            await _tareas.InsertOneAsync(t);

        public async Task UpdateAsync(string id, Tareas t) =>
            await _tareas.ReplaceOneAsync(x => x._id == ObjectId.Parse(id), t);

        public async Task DeleteAsync(string id) =>
            await _tareas.DeleteOneAsync(t => t._id == ObjectId.Parse(id));

        public async Task<List<Tareas>> GetTareasFromIdsAsync(List<ObjectId> ids)
        {
            if (ids == null || !ids.Any())
                return new List<Tareas>();

            var filter = Builders<Tareas>.Filter.In(t => t._id, ids);
            return await _tareas.Find(filter).ToListAsync();
        }
    }
}