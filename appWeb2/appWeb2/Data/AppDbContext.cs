using appWeb2.Models;
using Microsoft.EntityFrameworkCore;

namespace appWeb2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<VideoJuegos> VideoJuegos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Mapear tabla VideoJuegos
            modelBuilder.Entity<VideoJuegos>(entity =>
            {
                entity.ToTable("VideoJuegos");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.precio).HasColumnType("decimal(10,2)");
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()");
                entity.Property(e => e.enPromocion).HasDefaultValue(false);
                entity.Property(e => e.edadMinima).HasDefaultValue(0);
            });

            // Mapear tabla Usuarios
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.correo).IsUnique();
                entity.Property(e => e.password).HasColumnType("varbinary(64)");
                entity.Property(e => e.FechaRegistro).HasDefaultValueSql("GETDATE()");
            });
        }
    }
}
