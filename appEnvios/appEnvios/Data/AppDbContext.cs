using appEnvios.Models;
using Microsoft.EntityFrameworkCore;
namespace appEnvios.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { }
        public DbSet<Envio> Envio { get; set; }
        public DbSet<EstadoEnvio> EstadoEnvio { get; set; }
        public DbSet<Destinatario> Destinatarios { get; set; }
            public DbSet<Paquete> Paquetes { get; set; }
        public DbSet<Cliente> Clientes { get; set; }

        
    }
}
