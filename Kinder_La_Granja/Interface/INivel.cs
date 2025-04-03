using Kinder_La_Granja.Models;
using MongoDB.Driver;

namespace Kinder_La_Granja.Interface;

public interface INivel
{
    Task<List<Nivel>> GetAllAsync();
}