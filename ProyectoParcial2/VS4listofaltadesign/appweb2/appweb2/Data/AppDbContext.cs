using appweb2.Models;
using Microsoft.EntityFrameworkCore;

namespace appweb2.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set;  }
        public DbSet<Role> Roles { get; set; }
        public DbSet<VideoJuego> VideoJuegos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Compra> Compras { get; set; }
        public DbSet<DetalleCompra> DetalleCompras { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Usuario>().ToTable("Usuarios");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Compra>().ToTable("Compras");
            modelBuilder.Entity<VideoJuego>().ToTable("VideoJuegos", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_VideoJuegos_Precio_MayorACero", "[Precio] > 0");
                tableBuilder.HasCheckConstraint("CK_VideoJuegos_PrecioPromocion_Valido", "[PrecioPromocion] IS NULL OR ([PrecioPromocion] > 0 AND [PrecioPromocion] < [Precio])");
                tableBuilder.HasCheckConstraint("CK_VideoJuegos_EdadMinima_Valida", "[EdadMinima] BETWEEN 0 AND 21");
            });
            modelBuilder.Entity<Categoria>().ToTable("Categorias", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_Categorias_Nombre_NoVacio", "LEN(LTRIM(RTRIM([Nombre]))) > 0");
            });
            modelBuilder.Entity<DetalleCompra>().ToTable("detalle_compra", tableBuilder =>
            {
                tableBuilder.HasCheckConstraint("CK_DetalleCompra_Cantidad_Positiva", "[cantidad] > 0");
                tableBuilder.HasCheckConstraint("CK_DetalleCompra_Total_NoNegativo", "[total] >= 0");
            });

            modelBuilder.Entity<Usuario>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Categoria>()
                .HasIndex(c => c.Nombre)
                .IsUnique();

            modelBuilder.Entity<Usuario>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Usuarios)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<VideoJuego>()
                .HasOne(v => v.Categoria)
                .WithMany(c => c.VideoJuegos)
                .HasForeignKey(v => v.CategoriaId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Compra>()
                .HasOne(c => c.Usuario)
                .WithMany(u => u.compras)
                .HasForeignKey(c => c.UsuarioID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Compra>()
                .HasOne(c => c.VideoJuego)
                .WithMany()
                .HasForeignKey(c => c.VideoJuegoID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<DetalleCompra>()
                .HasOne(d => d.Compra)
                .WithMany(c => c.Detalles)
                .HasForeignKey(d => d.idCompra)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DetalleCompra>()
                .Property(d => d.VideoJuegosId)
                .HasMaxLength(200);

            modelBuilder.Entity<DetalleCompra>()
                .Property(d => d.estadoCompra)
                .HasMaxLength(50);

            modelBuilder.Entity<DetalleCompra>()
                .Property(d => d.codigoTransaccion)
                .HasMaxLength(100);
        }
    }

}
