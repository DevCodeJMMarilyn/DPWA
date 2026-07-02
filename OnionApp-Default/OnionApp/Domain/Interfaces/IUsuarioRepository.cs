using Domain.Entities;

namespace Domain.Interfaces;

public interface IUsuarioRepository
{
    Task<List<Usuario>> ObtenerTodos();
    Task<Usuario?> ObtenerPorId(int id);
    Task<Usuario?> ObtenerPorEmail(string email);
    Task Crear(Usuario usuario);
    Task Actualizar(Usuario usuario);
    Task Guardar();
}