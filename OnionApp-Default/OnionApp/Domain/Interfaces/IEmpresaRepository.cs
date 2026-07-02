using Domain.Entities;

namespace Domain.Interfaces;

public interface IEmpresaRepository
{
    Task<List<EmpresaCliente>> ObtenerTodas();
    Task<EmpresaCliente?> ObtenerPorId(int id);
    Task Crear(EmpresaCliente empresa);
    Task Actualizar(EmpresaCliente empresa);
    Task Guardar();
}