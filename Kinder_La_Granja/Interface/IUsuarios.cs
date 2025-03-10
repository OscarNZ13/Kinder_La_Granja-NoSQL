using Kinder_La_Granja.Models;
using MongoDB.Driver;

namespace Kinder_La_Granja.Interface;

public interface IUsuarios
{
    IMongoCollection<Usuarios> Usuarios { get; }

    IEnumerable<Usuarios> GetAllUsuarios();

    Usuarios GetUsuarioById(string id);

    Usuarios GetUsuarioByEmail(string email);

    void CreateUsuario(Usuarios usuario);

    void UpdateUsuario(string id, Usuarios usuario);

    void DeleteUsuario(string id);
}