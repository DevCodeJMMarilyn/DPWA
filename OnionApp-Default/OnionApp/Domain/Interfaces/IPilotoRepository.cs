using Domain.Entities;

namespace Domain.Interfaces;

public interface IPilotoRepository
{
    Task<List<Piloto>> ObtenerTodos();
    Task<Piloto?> ObtenerPorId(int id);
    Task Crear(Piloto piloto);
    Task Actualizar(Piloto piloto);
    Task Guardar();
}