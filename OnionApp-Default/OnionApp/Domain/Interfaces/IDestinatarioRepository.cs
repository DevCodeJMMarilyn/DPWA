using Domain.Entities;

namespace Domain.Interfaces;

public interface IDestinatarioRepository
{
    Task<List<Destinatario>> ObtenerTodos();
    Task<Destinatario?> ObtenerPorId(int id);
    Task Crear(Destinatario destinatario);
    Task Actualizar(Destinatario destinatario);
    Task Guardar();
}