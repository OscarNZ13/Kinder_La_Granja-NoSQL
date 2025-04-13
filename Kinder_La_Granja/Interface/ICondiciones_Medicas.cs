using Kinder_La_Granja.Models;
using MongoDB.Driver;

namespace Kinder_La_Granja.Interface;

public interface ICondiciones_Medicas
{
    IMongoCollection<Condiciones_Medicas> Condiciones_Medicas { get; }

    IEnumerable<Condiciones_Medicas> GetAllCondiciones_Medicas();

    void CreateCondiciones_Medicas(Condiciones_Medicas Condiciones_Medicas);

    Condiciones_Medicas GetCondiciones_MedicasById(string id);

    void UpdateCondiciones_Medicas(string id, Condiciones_Medicas Condiciones_Medicas);

    void DeleteCondiciones_Medicas(string id);

   
}



