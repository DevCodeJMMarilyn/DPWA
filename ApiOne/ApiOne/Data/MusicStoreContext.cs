using ApiOne.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiOne.Data
{
    // Contexto de Entity Framework para conectar la tienda de musica con la base de datos.
    public class MusicStoreContext : DbContext
    {
        public MusicStoreContext(DbContextOptions<MusicStoreContext> options)
            : base(options)
        {
        }

        public DbSet<MusicItem> MusicItems { get; set; }
    }
}
