using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Models;
using ProyectoProgramacionG7.Api.Data;

namespace ProyectoProgramacionG7.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComerciosController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComerciosController(AppDbContext context)
        {
            _context = context;
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
            comercio.FechaDeRegistro = DateTime.Now;
            _context.Comercios.Add(comercio);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetComercio), new { id = comercio.IdComercio }, comercio);
        }

        // PUT: api/Comercios/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutComercio(int id, Comercio comercio)
        {
            if (id != comercio.IdComercio)
                return BadRequest();

            comercio.FechaDeModificacion = DateTime.Now;
            _context.Entry(comercio).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Comercios.Any(e => e.IdComercio == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }

        // DELETE: api/Comercios/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteComercio(int id)
        {
            var comercio = await _context.Comercios.FindAsync(id);
            if (comercio == null)
                return NotFound();

            _context.Comercios.Remove(comercio);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}