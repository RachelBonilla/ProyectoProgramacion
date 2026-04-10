using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Models;
using ProyectoProgramacionG7.Api.Data;
using ProyectoProgramacionG7.Api.Services;
using System.Text.Json;

namespace ProyectoProgramacionG7.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComerciosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacora;

        public ComerciosController(AppDbContext context, IBitacoraService bitacora)
        {
            _context = context;
            _bitacora = bitacora;
        }

        // GET: api/Comercios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Comercio>>> GetComercios()
        {
            return await _context.Comercios.ToListAsync();
        }

        // GET: api/Comercios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Comercio>> GetComercio(int id)
        {
            var comercio = await _context.Comercios.FindAsync(id);

            if (comercio == null)
                return NotFound();

            return comercio;
        }

        // POST: api/Comercios
        [HttpPost]
        public async Task<ActionResult<Comercio>> PostComercio(Comercio comercio)
        {
            try
            {
                // Validar identificación duplicada
                bool existe = await _context.Comercios
                    .AnyAsync(c => c.Identificacion == comercio.Identificacion);

                if (existe)
                    return Conflict(new { mensaje = "Ya existe un comercio con esta identificación." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                comercio.FechaDeRegistro = DateTime.Now;
                comercio.Estado = true;

                _context.Comercios.Add(comercio);
                await _context.SaveChangesAsync();

                var datos = JsonSerializer.Serialize(comercio);

                await _bitacora.RegistrarEvento(
                    "Comercios",
                    "Registrar",
                    "Se registró un comercio desde la API",
                    "",
                    datos,
                    null
                );

                return CreatedAtAction(nameof(GetComercio), new { id = comercio.IdComercio }, comercio);
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Comercios",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );
                return StatusCode(500, new { mensaje = "Error al registrar el comercio.", error = ex.Message });
            }
        }

        // PUT: api/Comercios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComercio(int id, Comercio comercio)
        {
            if (id != comercio.IdComercio)
                return BadRequest(new { mensaje = "El ID no coincide." });

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var comercioAnterior = await _context.Comercios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.IdComercio == id);

                if (comercioAnterior == null)
                    return NotFound();

                var datosAnteriores = JsonSerializer.Serialize(comercioAnterior);

                comercio.FechaDeModificacion = DateTime.Now;

                _context.Comercios.Update(comercio);
                await _context.SaveChangesAsync();

                var datosPosteriores = JsonSerializer.Serialize(comercio);

                await _bitacora.RegistrarEvento(
                    "Comercios",
                    "Editar",
                    "Se editó un comercio desde la API",
                    "",
                    datosAnteriores,
                    datosPosteriores
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Comercios",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );
                return StatusCode(500, new { mensaje = "Error al editar el comercio.", error = ex.Message });
            }
        }

        // DELETE: api/Comercios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComercio(int id)
        {
            try
            {
                var comercio = await _context.Comercios.FindAsync(id);
                if (comercio == null)
                    return NotFound();

                var datos = JsonSerializer.Serialize(comercio);

                _context.Comercios.Remove(comercio);
                await _context.SaveChangesAsync();

                await _bitacora.RegistrarEvento(
                    "Comercios",
                    "Eliminar",
                    "Se eliminó un comercio desde la API",
                    "",
                    datos,
                    null
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Comercios",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );
                return StatusCode(500, new { mensaje = "Error al eliminar el comercio.", error = ex.Message });
            }
        }
    }
}