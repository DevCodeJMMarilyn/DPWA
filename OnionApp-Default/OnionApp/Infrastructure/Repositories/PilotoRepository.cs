using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PilotoRepository : IPilotoRepository
{
    private readonly AppDbContext _context;

    public PilotoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Piloto>> ObtenerTodos()
    {
        return await _context.Pilotos
            .Include(p => p.Usuario)
            .ToListAsync();
    }

    public async Task<Piloto?> ObtenerPorId(int id)
    {
        return await _context.Pilotos
            .Include(p => p.Usuario)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task Crear(Piloto piloto)
    {
        await _context.Pilotos.AddAsync(piloto);
    }

    public Task Actualizar(Piloto piloto)
    {
        _context.Pilotos.Update(piloto);
        return Task.CompletedTask;
    }

    public async Task Guardar()
    {
        await _context.SaveChangesAsync();
    }
}