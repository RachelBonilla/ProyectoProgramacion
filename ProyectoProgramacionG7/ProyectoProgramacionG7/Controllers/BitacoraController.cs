using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionG7.Data;

namespace ProyectoProgramacionG7.Controllers
{
    public class BitacoraController : Controller
    {
        private readonly AppDbContext _context;

        public BitacoraController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var eventos = await _context.BitacoraEventos
                .OrderByDescending(b => b.FechaDeEvento)
                .ToListAsync();

            return View(eventos);
        }
    }
}