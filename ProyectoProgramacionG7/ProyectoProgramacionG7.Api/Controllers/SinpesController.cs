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
    public class SinpesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacora;

        public SinpesController(AppDbContext context, IBitacoraService bitacora)
        {
            _context = context;
            _bitacora = bitacora;
        }

        // GET: api/Sinpes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sinpe>>> GetSinpes()
        {
            return await _context.Sinpes
                .Include(s => s.Caja)
                .ThenInclude(c => c.Comercio)
                .ToListAsync();
        }

        // GET: api/Sinpes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Sinpe>> GetSinpe(int id)
        {
            var sinpe = await _context.Sinpes
                .Include(s => s.Caja)
                .FirstOrDefaultAsync(s => s.IdSinpe == id);

            if (sinpe == null)
                return NotFound();

            return sinpe;
        }

        // POST: api/Sinpes
        [HttpPost]
        public async Task<ActionResult<Sinpe>> PostSinpe(Sinpe sinpe)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var caja = await _context.Cajas
                    .FirstOrDefaultAsync(c => c.IdCaja == sinpe.CajaId);

                if (caja == null || !caja.Estado)
                    return BadRequest(new { mensaje = "La caja no existe o está inactiva." });

                if (caja.TelefonoSINPE != sinpe.TelefonoDestinatario)
                    return BadRequest(new { mensaje = "El teléfono destinatario no coincide con la caja." });

                sinpe.FechaDeRegistro = DateTime.Now;
                sinpe.Estado = false; // No sincronizado

                _context.Sinpes.Add(sinpe);
                await _context.SaveChangesAsync();

                var datos = JsonSerializer.Serialize(sinpe);

                await _bitacora.RegistrarEvento(
                    "SINPE",
                    "Registrar",
                    "Se registró un pago SINPE desde la API",
                    "",
                    datos,
                    null
                );

                return CreatedAtAction(nameof(GetSinpe), new { id = sinpe.IdSinpe }, sinpe);
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "SINPE", "Error", ex.Message, ex.StackTrace, null, null);
                return StatusCode(500, new { mensaje = "Error al registrar SINPE.", error = ex.Message });
            }
        }
    }
}