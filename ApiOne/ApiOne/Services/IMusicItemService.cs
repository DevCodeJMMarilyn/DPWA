using ApiOne.Models;

namespace ApiOne.Services
{
    public interface IMusicItemService
    {
        Task<List<MusicItem>> ObtenerTodos();

        Task<MusicItem?> ObtenerPorId(int id);

        Task<MusicItem> Crear(MusicItem musicItem);

        Task<MusicItem?> Actualizar(int id, MusicItem musicItem);

        Task<bool> Eliminar(int id);
    }
}
