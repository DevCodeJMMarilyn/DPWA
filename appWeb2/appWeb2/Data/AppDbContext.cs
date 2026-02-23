using appWeb2.Models;
using Microsoft.EntityFrameworkCore;

namespace appWeb2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) 
            : base(options)
        {}
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<VideoJuegos> VideoJuegos { get; set; }
        public DbSet<Compra> Compras { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>()
                    .HasIndex(u => u.correo)
                    .IsUnique();

            modelBuilder.Entity<Compra>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.Compras)
                .HasForeignKey(c => c.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Compra>()
                    .HasOne(c => c.VideoJuegos)
                    .WithMany(v => v.Compras)
                    .HasForeignKey(c => c.VideoJuegosId)
                    .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
