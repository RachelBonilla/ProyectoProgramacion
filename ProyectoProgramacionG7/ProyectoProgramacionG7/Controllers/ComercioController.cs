using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionG7.Data;
using Modelos.Models;

namespace ProyectoProgramacionG7.Controllers
{
    public class ComercioController : Controller
    {
        private readonly AppDbContext _context;

        public ComercioController(AppDbContext context)
        {
            _context = context;
        }

        // LISTAR
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comercios.ToListAsync());
        }

        // DETALLES
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var comercio = await _context.Comercios
                .FirstOrDefaultAsync(c => c.IdComercio == id);

            if (comercio == null)
                return NotFound();

            return View(comercio);
        }

        // REGISTRAR (GET)
        public IActionResult Create()
        {
            return View();
        }

        // REGISTRAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comercio comercio)
        {
            // Validar identificación única
            bool existe = await _context.Comercios
                .AnyAsync(c => c.Identificacion == comercio.Identificacion);

            if (existe)
            {
                ModelState.AddModelError("Identificacion", "Ya existe un comercio con esta identificación.");
                return View(comercio);
            }

            if (ModelState.IsValid)
            {
                comercio.FechaDeRegistro = DateTime.Now;
                comercio.Estado = true;

                _context.Comercios.Add(comercio);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(comercio);
        }

        // EDITAR (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var comercio = await _context.Comercios.FindAsync(id);

            if (comercio == null)
                return NotFound();

            return View(comercio);
        }

        // EDITAR (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Comercio comercio)
        {
            if (id != comercio.IdComercio)
                return NotFound();

            if (ModelState.IsValid)
            {
                comercio.FechaDeModificacion = DateTime.Now;

                _context.Comercios.Update(comercio);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(comercio);
        }
    }
}