using ApiOne.Models;
using ApiOne.Repositories;

namespace ApiOne.Services
{
    public class MusicItemService : IMusicItemService
    {
        private readonly IMusicItemRepository _repository;

        public MusicItemService(IMusicItemRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<MusicItem>> ObtenerTodos()
        {
            return await _repository.ObtenerTodos();
        }

        public async Task<MusicItem?> ObtenerPorId(int id)
        {
            return await _repository.ObtenerPorId(id);
        }

        public async Task<MusicItem> Crear(MusicItem musicItem)
        {
            return await _repository.Crear(musicItem);
        }

        public async Task<MusicItem?> Actualizar(int id, MusicItem musicItem)
        {
            var existente = await _repository.ObtenerPorId(id);

            if (existente == null)
                return null;

            existente.Title = musicItem.Title;
            existente.Artist = musicItem.Artist;
            existente.Genre = musicItem.Genre;
            existente.ReleaseYear = musicItem.ReleaseYear;
            existente.Price = musicItem.Price;
            existente.Stock = musicItem.Stock;

            return await _repository.Actualizar(existente);
        }

        public async Task<bool> Eliminar(int id)
        {
            return await _repository.Eliminar(id);
        }
    }
}
