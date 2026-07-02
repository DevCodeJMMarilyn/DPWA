using ApiOne.Models;

namespace ApiOne.Repositories
{
    public interface IMusicItemRepository
    {
        Task<List<MusicItem>> ObtenerTodos();

        Task<MusicItem?> ObtenerPorId(int id);

        Task<MusicItem> Crear(MusicItem musicItem);

        Task<MusicItem> Actualizar(MusicItem musicItem);

        Task<bool> Eliminar(int id);
    }
}
