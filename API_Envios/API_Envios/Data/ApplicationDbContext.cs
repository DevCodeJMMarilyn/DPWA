using System.Text.Json;
using API_Envios.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace API_Envios.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Empresa> Empresas => Set<Empresa>();
    public DbSet<Piloto> Pilotos => Set<Piloto>();
    public DbSet<Destinatario> Destinatarios => Set<Destinatario>();
    public DbSet<Envio> Envios => Set<Envio>();
    public DbSet<HistorialEnvio> HistorialEnvios => Set<HistorialEnvio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasIndex(u => u.Correo).IsUnique();
            entity.Property(u => u.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(u => u.Correo).HasMaxLength(160).IsRequired();
            entity.Property(u => u.Password).HasMaxLength(100).IsRequired();
            entity.Property(u => u.Rol).HasConversion<string>().HasMaxLength(40);
            entity.HasOne<Empresa>().WithMany().HasForeignKey(u => u.EmpresaId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Piloto>().WithMany().HasForeignKey(u => u.PilotoId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Empresa>(entity =>
        {
            entity.Property(e => e.Nombre).HasMaxLength(160).IsRequired();
            entity.Property(e => e.Nit).HasMaxLength(40).IsRequired();
            entity.Property(e => e.Telefono).HasMaxLength(20).IsRequired();
            entity.Property(e => e.Correo).HasMaxLength(160).IsRequired();
            entity.Property(e => e.Departamento).HasMaxLength(80).IsRequired();
            entity.Property(e => e.Distrito).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Direccion).HasMaxLength(250).IsRequired();
        });

        modelBuilder.Entity<Piloto>(entity =>
        {
            entity.Property(p => p.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(p => p.Documento).HasMaxLength(30).IsRequired();
            entity.Property(p => p.Telefono).HasMaxLength(20).IsRequired();
            entity.Property(p => p.Departamento).HasMaxLength(80).IsRequired();
            entity.Property(p => p.Distrito).HasMaxLength(100).IsRequired();
        });

        modelBuilder.Entity<Destinatario>(entity =>
        {
            entity.Property(d => d.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(d => d.Telefono).HasMaxLength(20).IsRequired();
            entity.Property(d => d.Departamento).HasMaxLength(80).IsRequired();
            entity.Property(d => d.Distrito).HasMaxLength(100).IsRequired();
            entity.Property(d => d.Direccion).HasMaxLength(250).IsRequired();
            entity.HasOne<Empresa>().WithMany().HasForeignKey(d => d.EmpresaId).OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Envio>(entity =>
        {
            entity.HasIndex(e => e.CodigoRastreo).IsUnique();
            entity.Property(e => e.CodigoRastreo).HasMaxLength(40).IsRequired();
            entity.Property(e => e.DescripcionPedido).HasMaxLength(500).IsRequired();
            entity.Property(e => e.PesoLibras).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Estado).HasConversion<string>().HasMaxLength(30);
            entity.Property(e => e.FirmaRecibido).HasMaxLength(250);
            entity.Property(e => e.ImagenesEntrega)
                .HasConversion(
                    value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null),
                    value => JsonSerializer.Deserialize<List<string>>(value, (JsonSerializerOptions?)null) ?? new List<string>())
                .Metadata.SetValueComparer(new ValueComparer<List<string>>(
                    (left, right) => JsonSerializer.Serialize(left, (JsonSerializerOptions?)null) == JsonSerializer.Serialize(right, (JsonSerializerOptions?)null),
                    value => JsonSerializer.Serialize(value, (JsonSerializerOptions?)null).GetHashCode(),
                    value => JsonSerializer.Deserialize<List<string>>(JsonSerializer.Serialize(value, (JsonSerializerOptions?)null), (JsonSerializerOptions?)null) ?? new List<string>()));
            entity.HasOne<Empresa>().WithMany().HasForeignKey(e => e.EmpresaId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Destinatario>().WithMany().HasForeignKey(e => e.DestinatarioId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne<Piloto>().WithMany().HasForeignKey(e => e.PilotoId).OnDelete(DeleteBehavior.Restrict);
            entity.HasMany(e => e.Historial).WithOne().HasForeignKey(h => h.EnvioId).OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HistorialEnvio>(entity =>
        {
            entity.Property(h => h.Estado).HasConversion<string>().HasMaxLength(30);
            entity.Property(h => h.Comentario).HasMaxLength(500).IsRequired();
            entity.Property(h => h.Usuario).HasMaxLength(120).IsRequired();
            entity.HasOne<Usuario>().WithMany().HasForeignKey(h => h.UsuarioId).OnDelete(DeleteBehavior.SetNull);
        });

        Seed(modelBuilder);
    }

    private static void Seed(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Empresa>().HasData(new Empresa
        {
            Id = 1,
            Nombre = "Mishi Store",
            Nit = "0614-260526-101-1",
            Telefono = "77777777",
            Correo = "empresa@envios.test",
            Departamento = "San Salvador",
            Distrito = "San Salvador Centro",
            Direccion = "Colonia Escalon",
            Activa = true
        });

        modelBuilder.Entity<Piloto>().HasData(new Piloto
        {
            Id = 1,
            Nombre = "Piloto Demo",
            Documento = "00000000-0",
            Telefono = "77770000",
            Departamento = "San Salvador",
            Distrito = "San Salvador Centro",
            Activo = true
        });

        modelBuilder.Entity<Usuario>().HasData(
            new Usuario { Id = 1, Nombre = "Administrador Master", Correo = "master@envios.test", Password = "Master123", Rol = RolUsuario.AdministradorMaster, Activo = true },
            new Usuario { Id = 2, Nombre = "Admin San Salvador", Correo = "admin@envios.test", Password = "Admin123", Rol = RolUsuario.Administrador, Activo = true, Departamento = "San Salvador", Distrito = "San Salvador Centro", DireccionCercana = "Centro" },
            new Usuario { Id = 3, Nombre = "Mishi Store", Correo = "empresa@envios.test", Password = "Empresa123", Rol = RolUsuario.EmpresaCliente, Activo = true, EmpresaId = 1 },
            new Usuario { Id = 4, Nombre = "Piloto Demo", Correo = "piloto@envios.test", Password = "Piloto123", Rol = RolUsuario.Piloto, Activo = true, PilotoId = 1 });

        modelBuilder.Entity<Destinatario>().HasData(new Destinatario
        {
            Id = 1,
            EmpresaId = 1,
            Nombre = "Michelle Jimenez",
            Telefono = "77777777",
            Departamento = "San Salvador",
            Distrito = "San Salvador Centro",
            Direccion = "San Salvador"
        });

        modelBuilder.Entity<Envio>().HasData(new
        {
            Id = 1,
            CodigoRastreo = "ENV-20260526-0001",
            EmpresaId = 1,
            DestinatarioId = 1,
            PilotoId = (int?)1,
            DescripcionPedido = "Paquete de prueba",
            PesoLibras = 2.5m,
            Estado = EstadoEnvio.EnRuta,
            FechaCreacion = new DateTime(2026, 5, 26, 0, 0, 0, DateTimeKind.Utc),
            FechaEntrega = (DateTime?)null,
            FirmaRecibido = (string?)null,
            ImagenesEntrega = new List<string>()
        });

        modelBuilder.Entity<HistorialEnvio>().HasData(
            new { Id = 1, EnvioId = 1, UsuarioId = (int?)1, Fecha = new DateTime(2026, 5, 26, 0, 0, 0, DateTimeKind.Utc), Estado = EstadoEnvio.Recolectado, Comentario = "Envio creado", Usuario = "Sistema" },
            new { Id = 2, EnvioId = 1, UsuarioId = (int?)1, Fecha = new DateTime(2026, 5, 26, 0, 5, 0, DateTimeKind.Utc), Estado = EstadoEnvio.EnRuta, Comentario = "Asignado a piloto demo", Usuario = "Administrador Master" });
    }
}
