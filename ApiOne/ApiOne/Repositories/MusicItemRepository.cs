using ApiOne.Data;
using ApiOne.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiOne.Repositories
{
    public class MusicItemRepository : IMusicItemRepository
    {
        private readonly MusicStoreContext _context;

        public MusicItemRepository(MusicStoreContext context)
        {
            _context = context;
        }

        public async Task<List<MusicItem>> ObtenerTodos()
        {
            return await _context.MusicItems.ToListAsync();
        }

        public async Task<MusicItem?> ObtenerPorId(int id)
        {
            return await _context.MusicItems.FindAsync(id);
        }

        public async Task<MusicItem> Crear(MusicItem musicItem)
        {
            _context.MusicItems.Add(musicItem);

            await _context.SaveChangesAsync();

            return musicItem;
        }

        public async Task<MusicItem> Actualizar(MusicItem musicItem)
        {
            _context.MusicItems.Update(musicItem);

            await _context.SaveChangesAsync();

            return musicItem;
        }

        public async Task<bool> Eliminar(int id)
        {
            var musicItem = await _context.MusicItems.FindAsync(id);

            if (musicItem == null)
                return false;

            _context.MusicItems.Remove(musicItem);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}
