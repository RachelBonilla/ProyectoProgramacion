using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Models;
using ProyectoProgramacionG7.Api.Data;

namespace ProyectoProgramacionG7.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BitacoraController : ControllerBase
    {
        private readonly AppDbContext _context;

        public BitacoraController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Bitacora
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BitacoraEvento>>> GetEventos()
        {
            return await _context.BitacoraEventos
                .OrderByDescending(b => b.FechaDeEvento)
                .ToListAsync();
        }

        // GET: api/Bitacora/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BitacoraEvento>> GetEvento(int id)
        {
            var evento = await _context.BitacoraEventos.FindAsync(id);

            if (evento == null)
                return NotFound();

            return evento;
        }
    }
}