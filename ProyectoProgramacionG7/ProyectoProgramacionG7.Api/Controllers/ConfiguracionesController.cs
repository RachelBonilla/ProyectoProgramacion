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
    public class ConfiguracionesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacora;

        public ConfiguracionesController(AppDbContext context, IBitacoraService bitacora)
        {
            _context = context;
            _bitacora = bitacora;
        }

        // GET: api/Configuraciones
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ConfiguracionComercio>>> GetConfiguraciones()
        {
            return await _context.Configuraciones
                .Include(c => c.Comercio)
                .ToListAsync();
        }

        // GET: api/Configuraciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ConfiguracionComercio>> GetConfiguracion(int id)
        {
            var config = await _context.Configuraciones
                .Include(c => c.Comercio)
                .FirstOrDefaultAsync(c => c.IdConfiguracion == id);

            if (config == null)
                return NotFound();

            return config;
        }

        // POST: api/Configuraciones
        [HttpPost]
        public async Task<ActionResult<ConfiguracionComercio>> PostConfiguracion(ConfiguracionComercio config)
        {
            try
            {
                bool existe = await _context.Configuraciones
                    .AnyAsync(c => c.IdComercio == config.IdComercio);

                if (existe)
                    return Conflict(new { mensaje = "Ya existe una configuración para este comercio." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                config.FechaDeRegistro = DateTime.Now;
                config.Estado = true;

                _context.Configuraciones.Add(config);
                await _context.SaveChangesAsync();

                var datos = JsonSerializer.Serialize(config);

                await _bitacora.RegistrarEvento(
                    "Configuracion",
                    "Registrar",
                    "Se creó configuración desde la API",
                    "",
                    datos,
                    null
                );

                return CreatedAtAction(nameof(GetConfiguracion), new { id = config.IdConfiguracion }, config);
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Configuracion", "Error", ex.Message, ex.StackTrace, null, null);
                return StatusCode(500, new { mensaje = "Error al registrar configuración.", error = ex.Message });
            }
        }

        // PUT: api/Configuraciones/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutConfiguracion(int id, ConfiguracionComercio config)
        {
            if (id != config.IdConfiguracion)
                return BadRequest(new { mensaje = "El ID no coincide." });

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var anterior = await _context.Configuraciones
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.IdConfiguracion == id);

                if (anterior == null)
                    return NotFound();

                var datosAntes = JsonSerializer.Serialize(anterior);

                config.FechaDeModificacion = DateTime.Now;

                _context.Update(config);
                await _context.SaveChangesAsync();

                var datosDespues = JsonSerializer.Serialize(config);

                await _bitacora.RegistrarEvento(
                    "Configuracion",
                    "Editar",
                    "Se editó configuración desde la API",
                    "",
                    datosAntes,
                    datosDespues
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Configuracion", "Error", ex.Message, ex.StackTrace, null, null);
                return StatusCode(500, new { mensaje = "Error al editar configuración.", error = ex.Message });
            }
        }

        // DELETE: api/Configuraciones/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteConfiguracion(int id)
        {
            try
            {
                var config = await _context.Configuraciones.FindAsync(id);
                if (config == null)
                    return NotFound();

                var datos = JsonSerializer.Serialize(config);

                _context.Configuraciones.Remove(config);
                await _context.SaveChangesAsync();

                await _bitacora.RegistrarEvento(
                    "Configuracion",
                    "Eliminar",
                    "Se eliminó configuración desde la API",
                    "",
                    datos,
                    null
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Configuracion", "Error", ex.Message, ex.StackTrace, null, null);
                return StatusCode(500, new { mensaje = "Error al eliminar configuración.", error = ex.Message });
            }
        }
    }
}