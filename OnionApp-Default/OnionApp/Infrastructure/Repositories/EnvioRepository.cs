using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EnvioRepository : IEnvioRepository
{
    private readonly AppDbContext _context;

    public EnvioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Envio>> ObtenerTodos()
    {
        return await _context.Envios
            .Include(e => e.EmpresaCliente)
            .Include(e => e.Destinatario)
            .Include(e => e.Piloto)
                .ThenInclude(p => p!.Usuario)
            .Include(e => e.Entrega)
            .ToListAsync();
    }

    public async Task<Envio?> ObtenerPorId(int id)
    {
        return await _context.Envios
            .Include(e => e.EmpresaCliente)
            .Include(e => e.Destinatario)
            .Include(e => e.Piloto)
                .ThenInclude(p => p!.Usuario)
            .Include(e => e.Entrega)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task Crear(Envio envio)
    {
        await _context.Envios.AddAsync(envio);
    }

    public Task Actualizar(Envio envio)
    {
        _context.Envios.Update(envio);
        return Task.CompletedTask;
    }

    public async Task CrearEntrega(Entrega entrega)
    {
        await _context.Entregas.AddAsync(entrega);
    }

    public async Task Guardar()
    {
        await _context.SaveChangesAsync();
    }
}