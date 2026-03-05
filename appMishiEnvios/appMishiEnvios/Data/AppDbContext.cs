using appMishiEnvios.Models;
using Microsoft.EntityFrameworkCore;

namespace appMishiEnvios.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {}

        public DbSet<Cliente> Clientes { get; set; }

        public DbSet<Destinatario> Destinatarios { get; set; }

        public DbSet<EstadoEnvio> EstadosEnvio { get; set; }

        public DbSet<Envio> Envios { get; set; }

        public DbSet<Auditoria> Auditorias { get; set; }
    }
}