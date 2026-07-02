using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class EmpresaRepository : IEmpresaRepository
{
    private readonly AppDbContext _context;

    public EmpresaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<EmpresaCliente>> ObtenerTodas()
    {
        return await _context.EmpresasClientes
            .Include(e => e.Usuario)
            .ToListAsync();
    }

    public async Task<EmpresaCliente?> ObtenerPorId(int id)
    {
        return await _context.EmpresasClientes
            .Include(e => e.Usuario)
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task Crear(EmpresaCliente empresa)
    {
        await _context.EmpresasClientes.AddAsync(empresa);
    }

    public Task Actualizar(EmpresaCliente empresa)
    {
        _context.EmpresasClientes.Update(empresa);
        return Task.CompletedTask;
    }

    public async Task Guardar()
    {
        await _context.SaveChangesAsync();
    }
}