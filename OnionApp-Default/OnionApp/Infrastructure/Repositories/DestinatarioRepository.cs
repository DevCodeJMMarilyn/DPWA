using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class DestinatarioRepository : IDestinatarioRepository
{
    private readonly AppDbContext _context;

    public DestinatarioRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Destinatario>> ObtenerTodos()
    {
        return await _context.Destinatarios
            .Include(d => d.EmpresaCliente)
            .ToListAsync();
    }

    public async Task<Destinatario?> ObtenerPorId(int id)
    {
        return await _context.Destinatarios
            .Include(d => d.EmpresaCliente)
            .FirstOrDefaultAsync(d => d.Id == id);
    }

    public async Task Crear(Destinatario destinatario)
    {
        await _context.Destinatarios.AddAsync(destinatario);
    }

    public Task Actualizar(Destinatario destinatario)
    {
        _context.Destinatarios.Update(destinatario);
        return Task.CompletedTask;
    }

    public async Task Guardar()
    {
        await _context.SaveChangesAsync();
    }
}