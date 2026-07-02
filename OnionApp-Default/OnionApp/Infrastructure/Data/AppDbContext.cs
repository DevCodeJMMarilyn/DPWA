using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<EmpresaCliente> EmpresasClientes => Set<EmpresaCliente>();
    public DbSet<Piloto> Pilotos => Set<Piloto>();
    public DbSet<Destinatario> Destinatarios => Set<Destinatario>();
    public DbSet<Envio> Envios => Set<Envio>();
    public DbSet<Entrega> Entregas => Set<Entrega>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Usuario>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.EmpresaCliente)
            .WithOne(e => e.Usuario)
            .HasForeignKey<EmpresaCliente>(e => e.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Usuario>()
            .HasOne(u => u.Piloto)
            .WithOne(p => p.Usuario)
            .HasForeignKey<Piloto>(p => p.UsuarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmpresaCliente>()
            .HasMany(e => e.Destinatarios)
            .WithOne(d => d.EmpresaCliente)
            .HasForeignKey(d => d.EmpresaClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmpresaCliente>()
            .HasMany(e => e.Envios)
            .WithOne(e => e.EmpresaCliente)
            .HasForeignKey(e => e.EmpresaClienteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Destinatario>()
            .HasMany(d => d.Envios)
            .WithOne(e => e.Destinatario)
            .HasForeignKey(e => e.DestinatarioId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Piloto>()
            .HasMany(p => p.EnviosAsignados)
            .WithOne(e => e.Piloto)
            .HasForeignKey(e => e.PilotoId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Envio>()
            .HasOne(e => e.Entrega)
            .WithOne(e => e.Envio)
            .HasForeignKey<Entrega>(e => e.EnvioId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}