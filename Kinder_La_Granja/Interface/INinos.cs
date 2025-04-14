using Kinder_La_Granja.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Kinder_La_Granja.Interface;

public interface INinos
{
    Task<List<Ninos>> GetAllAsync();
    Task<List<Ninos>> GetByNivelAsync(int nivelId);
    Task<Ninos> GetByIdAsync(ObjectId id);
    Task CreateAsync(Ninos nino);
    //Task<List<Ninos>> GetByCondiciones_MedicasAsync(string condicionMedicaId);
    Task UpdateAsync(ObjectId id, Ninos nino);
    Task DeleteAsync(ObjectId id);

}