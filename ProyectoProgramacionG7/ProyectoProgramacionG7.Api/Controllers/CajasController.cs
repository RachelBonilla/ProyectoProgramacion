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
    public class CajasController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacora;

        public CajasController(AppDbContext context, IBitacoraService bitacora)
        {
            _context = context;
            _bitacora = bitacora;
        }

        // GET: api/Cajas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Caja>>> GetCajas([FromQuery] int? comercioId = null)
        {
            var query = _context.Cajas.Include(c => c.Comercio).AsQueryable();

            if (comercioId != null)
                query = query.Where(c => c.IdComercio == comercioId);

            return await query.ToListAsync();
        }

        // GET: api/Cajas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Caja>> GetCaja(int id)
        {
            var caja = await _context.Cajas
                .Include(c => c.Comercio)
                .FirstOrDefaultAsync(c => c.IdCaja == id);

            if (caja == null)
                return NotFound();

            return caja;
        }

        // POST: api/Cajas
        [HttpPost]
        public async Task<ActionResult<Caja>> PostCaja(Caja caja)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                caja.FechaDeRegistro = DateTime.Now;
                caja.Estado = true;

                _context.Cajas.Add(caja);
                await _context.SaveChangesAsync();

                var datos = JsonSerializer.Serialize(caja);

                await _bitacora.RegistrarEvento(
                    "Cajas",
                    "Registrar",
                    "Se registró una caja desde la API",
                    "",
                    datos,
                    null
                );

                return CreatedAtAction(nameof(GetCaja), new { id = caja.IdCaja }, caja);
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Cajas", "Error", ex.Message, ex.StackTrace, null, null);
                return StatusCode(500, new { mensaje = "Error al registrar la caja.", error = ex.Message });
            }
        }

        // PUT: api/Cajas/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCaja(int id, Caja caja)
        {
            if (id != caja.IdCaja)
                return BadRequest(new { mensaje = "El ID no coincide." });

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var anterior = await _context.Cajas
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.IdCaja == id);

                if (anterior == null)
                    return NotFound();

                var datosAntes = JsonSerializer.Serialize(anterior);

                caja.FechaDeModificacion = DateTime.Now;

                _context.Update(caja);
                await _context.SaveChangesAsync();

                var datosDespues = JsonSerializer.Serialize(caja);

                await _bitacora.RegistrarEvento(
                    "Cajas",
                    "Editar",
                    "Se editó una caja desde la API",
                    "",
                    datosAntes,
                    datosDespues
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Cajas", "Error", ex.Message, ex.StackTrace, null, null);
                return StatusCode(500, new { mensaje = "Error al editar la caja.", error = ex.Message });
            }
        }

        // DELETE: api/Cajas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCaja(int id)
        {
            try
            {
                var caja = await _context.Cajas.FindAsync(id);
                if (caja == null)
                    return NotFound();

                var datos = JsonSerializer.Serialize(caja);

                _context.Cajas.Remove(caja);
                await _context.SaveChangesAsync();

                await _bitacora.RegistrarEvento(
                    "Cajas",
                    "Eliminar",
                    "Se eliminó una caja desde la API",
                    "",
                    datos,
                    null
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Cajas", "Error", ex.Message, ex.StackTrace, null, null);
                return StatusCode(500, new { mensaje = "Error al eliminar la caja.", error = ex.Message });
            }
        }

        // GET: api/Cajas/5/Sinpes
        [HttpGet("{id}/Sinpes")]
        public async Task<ActionResult<IEnumerable<Sinpe>>> GetSinpesDeCaja(int id)
        {
            var sinpes = await _context.Sinpes
                .Where(s => s.CajaId.HasValue && s.CajaId == id)
                .OrderByDescending(s => s.FechaDeRegistro)
                .ToListAsync();

            return sinpes;
        }

        // PUT: api/Cajas/Sincronizar/5
        [HttpPut("Sincronizar/{idSinpe}")]
        public async Task<IActionResult> Sincronizar(int idSinpe)
        {
            var sinpe = await _context.Sinpes.FindAsync(idSinpe);

            if (sinpe == null)
                return NotFound();

            if (sinpe.Estado == true)
                return Ok(new { mensaje = "El SINPE ya estaba sincronizado." });

            sinpe.Estado = true;
            await _context.SaveChangesAsync();

            return Ok(new { mensaje = "SINPE sincronizado correctamente." });
        }
    }
}