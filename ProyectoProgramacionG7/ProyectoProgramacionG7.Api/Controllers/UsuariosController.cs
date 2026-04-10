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
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacora;

        public UsuariosController(AppDbContext context, IBitacoraService bitacora)
        {
            _context = context;
            _bitacora = bitacora;
        }

        // GET: api/Usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            return await _context.Usuarios
                .Include(u => u.Comercio)
                .ToListAsync();
        }

        // GET: api/Usuarios/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Comercio)
                .FirstOrDefaultAsync(u => u.IdUsuario == id);

            if (usuario == null)
                return NotFound();

            return usuario;
        }

        // POST: api/Usuarios
        [HttpPost]
        public async Task<ActionResult<Usuario>> PostUsuario(Usuario usuario)
        {
            try
            {
                bool existe = await _context.Usuarios
                    .AnyAsync(u => u.Identificacion == usuario.Identificacion);

                if (existe)
                    return Conflict(new { mensaje = "Ya existe un usuario con esa identificación." });

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                usuario.FechaDeRegistro = DateTime.Now;
                usuario.Estado = true;

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                var datos = JsonSerializer.Serialize(usuario);

                await _bitacora.RegistrarEvento(
                    "Usuarios",
                    "Registrar",
                    "Se creó usuario desde la API",
                    "",
                    datos,
                    null
                );

                return CreatedAtAction(nameof(GetUsuario), new { id = usuario.IdUsuario }, usuario);
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Usuarios", "Error", ex.Message, ex.StackTrace, null, null);
                return StatusCode(500, new { mensaje = "Error al registrar usuario.", error = ex.Message });
            }
        }

        // PUT: api/Usuarios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUsuario(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return BadRequest(new { mensaje = "El ID no coincide." });

            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var anterior = await _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.IdUsuario == id);

                if (anterior == null)
                    return NotFound();

                var datosAntes = JsonSerializer.Serialize(anterior);

                usuario.FechaDeModificacion = DateTime.Now;

                _context.Update(usuario);
                await _context.SaveChangesAsync();

                var datosDespues = JsonSerializer.Serialize(usuario);

                await _bitacora.RegistrarEvento(
                    "Usuarios",
                    "Editar",
                    "Se editó usuario desde la API",
                    "",
                    datosAntes,
                    datosDespues
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Usuarios", "Error", ex.Message, ex.StackTrace, null, null);
                return StatusCode(500, new { mensaje = "Error al editar usuario.", error = ex.Message });
            }
        }

        // DELETE: api/Usuarios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            try
            {
                var usuario = await _context.Usuarios.FindAsync(id);
                if (usuario == null)
                    return NotFound();

                var datos = JsonSerializer.Serialize(usuario);

                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();

                await _bitacora.RegistrarEvento(
                    "Usuarios",
                    "Eliminar",
                    "Se eliminó usuario desde la API",
                    "",
                    datos,
                    null
                );

                return NoContent();
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Usuarios", "Error", ex.Message, ex.StackTrace, null, null);
                return StatusCode(500, new { mensaje = "Error al eliminar usuario.", error = ex.Message });
            }
        }
    }
}