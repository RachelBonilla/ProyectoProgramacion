using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Modelos.Models;
using ProyectoProgramacionG7.Api.Data;
using ProyectoProgramacionG7.Api.DataTransfer;
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

        // GET
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Sinpe>>> GetSinpes()
        {
            return await _context.Sinpes
                .Include(s => s.Caja)
                .ThenInclude(c => c.Comercio)
                .ToListAsync();
        }

        // GET (id)
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


        // GET SINPE POR TELÉFONO DE CAJA
        [Authorize] 
        [HttpGet("Consultar/{telefonoCaja}")]
        public async Task<ActionResult<IEnumerable<object>>> ConsultarSinpes(string telefonoCaja)
        {
            var caja = await _context.Cajas
                .Include(c => c.Comercio)
                .FirstOrDefaultAsync(c => c.TelefonoSINPE == telefonoCaja);

            if (caja == null)
                return NotFound(new ApiResponse
                {
                    EsValido = false,
                    Mensaje = "No existe una caja con ese número de teléfono."
                });

            var config = await _context.Configuraciones
                .FirstOrDefaultAsync(c => c.IdComercio == caja.IdComercio);

            if (config == null || (config.TipoConfiguracion != 2 && config.TipoConfiguracion != 3))
                return BadRequest(new ApiResponse
                {
                    EsValido = false,
                    Mensaje = "Este comercio no tiene permitido usar sincronización externa."
                });

            var sinpes = await _context.Sinpes
                .Where(s => s.CajaId == caja.IdCaja)
                .Select(s => new
                {
                    s.IdSinpe,
                    s.TelefonoOrigen,
                    s.NombreOrigen,
                    s.TelefonoDestinatario,
                    s.NombreDestinatario,
                    s.Monto,
                    s.Descripcion,
                    Fecha = s.FechaDeRegistro,
                    s.Estado
                })
                .ToListAsync();

            return Ok(sinpes);
        }

        // PUT-SINPE PARA SINCRONIZAR DESDE SISTEMA EXTERNO
        [Authorize]
        [HttpPut("Sincronizar/{idSinpe}")]
        public async Task<ActionResult<ApiResponse>> SincronizarSinpe(int idSinpe)
        {
            try
            {
                var sinpe = await _context.Sinpes
                    .Include(s => s.Caja)
                    .FirstOrDefaultAsync(s => s.IdSinpe == idSinpe);

                if (sinpe == null)
                    return NotFound(new ApiResponse
                    {
                        EsValido = false,
                        Mensaje = "El SINPE indicado no existe."
                    });

                var config = await _context.Configuraciones
                    .FirstOrDefaultAsync(c => c.IdComercio == sinpe.Caja!.IdComercio);

                if (config == null || (config.TipoConfiguracion != 2 && config.TipoConfiguracion != 3))
                    return BadRequest(new ApiResponse
                    {
                        EsValido = false,
                        Mensaje = "Este comercio no tiene permitido usar sincronización externa."
                    });

                if (sinpe.Estado == true)
                    return Ok(new ApiResponse
                    {
                        EsValido = true,
                        Mensaje = "El SINPE ya estaba sincronizado previamente."
                    });

                sinpe.Estado = true;
                await _context.SaveChangesAsync();

                await _bitacora.RegistrarEvento(
                    "SINPE",
                    "Sincronizar",
                    $"SINPE {idSinpe} sincronizado desde sistema externo",
                    "",
                    null,
                    JsonSerializer.Serialize(sinpe)
                );

                return Ok(new ApiResponse
                {
                    EsValido = true,
                    Mensaje = "SINPE sincronizado correctamente."
                });
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "SINPE", "Error", ex.Message, ex.StackTrace, null, null);

                return StatusCode(500, new ApiResponse
                {
                    EsValido = false,
                    Mensaje = $"Error al sincronizar: {ex.Message}"
                });
            }
        }

        // POST Recibir
        [Authorize]
        [HttpPost("Recibir")]
        public async Task<ActionResult<ApiResponse>> RecibirSinpe(Sinpe sinpe)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponse
                    {
                        EsValido = false,
                        Mensaje = "Datos inválidos en el SINPE recibido."
                    });

                var caja = await _context.Cajas
                    .FirstOrDefaultAsync(c => c.TelefonoSINPE == sinpe.TelefonoDestinatario);

                if (caja == null || !caja.Estado)
                    return BadRequest(new ApiResponse
                    {
                        EsValido = false,
                        Mensaje = "No existe una caja activa con el teléfono destinatario."
                    });

                sinpe.CajaId = caja.IdCaja;
                sinpe.FechaDeRegistro = DateTime.Now;
                sinpe.Estado = false;

                _context.Sinpes.Add(sinpe);
                await _context.SaveChangesAsync();

                await _bitacora.RegistrarEvento(
                    "SINPE",
                    "Recibir",
                    "SINPE recibido desde entidad financiera",
                    "",
                    JsonSerializer.Serialize(sinpe),
                    null
                );

                return Ok(new ApiResponse
                {
                    EsValido = true,
                    Mensaje = $"SINPE recibido y registrado correctamente con ID {sinpe.IdSinpe}."
                });
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "SINPE", "Error", ex.Message, ex.StackTrace, null, null);

                return StatusCode(500, new ApiResponse
                {
                    EsValido = false,
                    Mensaje = $"Error al recibir SINPE: {ex.Message}"
                });
            }
        }

        // POST Original
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
                sinpe.Estado = false;

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