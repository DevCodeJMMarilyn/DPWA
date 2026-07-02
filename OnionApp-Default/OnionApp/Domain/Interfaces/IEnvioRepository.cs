using Domain.Entities;

namespace Domain.Interfaces;

public interface IEnvioRepository
{
    Task<List<Envio>> ObtenerTodos();
    Task<Envio?> ObtenerPorId(int id);
    Task Crear(Envio envio);
    Task Actualizar(Envio envio);
    Task CrearEntrega(Entrega entrega);
    Task Guardar();
}