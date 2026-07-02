using ApiOne.Models;
using ApiOne.Services;
using Microsoft.AspNetCore.Mvc;

namespace ApiOne.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MusicItemsController : ControllerBase
    {
        private readonly IMusicItemService _service;

        public MusicItemsController(IMusicItemService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerTodos()
        {
            var musicItems = await _service.ObtenerTodos();

            return Ok(musicItems);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObtenerPorId(int id)
        {
            var musicItem = await _service.ObtenerPorId(id);

            if (musicItem == null)
                return NotFound();

            return Ok(musicItem);
        }

        [HttpPost]
        public async Task<IActionResult> Crear(MusicItem musicItem)
        {
            var nuevo = await _service.Crear(musicItem);

            return Ok(nuevo);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, MusicItem musicItem)
        {
            var actualizado = await _service.Actualizar(id, musicItem);

            if (actualizado == null)
                return NotFound();

            return Ok(actualizado);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var eliminado = await _service.Eliminar(id);

            if (!eliminado)
                return NotFound();

            return Ok("Articulo musical eliminado");
        }
    }
}
