using Kinder_La_Granja.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Threading;

namespace Kinder_La_Granja.Interface;

public interface ITareas
{
    Task<List<Tareas>> GetAllAsync();
    Task<Tareas> GetByIdAsync(string id);
    Task<List<Tareas>> GetByDocenteAsync(string docenteId);
    Task CreateAsync(Tareas t);
    Task UpdateAsync(string id, Tareas t);
    Task DeleteAsync(string id);
}