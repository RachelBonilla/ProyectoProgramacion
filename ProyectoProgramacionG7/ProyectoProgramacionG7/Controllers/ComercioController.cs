using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
using Modelos.Models;
using ProyectoProgramacionG7.Data;
=======
>>>>>>> 6ca8f85454cdf3d6862cb503bb68d15af25d95b6
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

<<<<<<< HEAD
        // 🔹 LISTAR
        public async Task<IActionResult> Index()
        {
            return View(await _context.Comercios.ToListAsync());
        }

        // 🔹 DETALLES
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

        // 🔹 REGISTRAR (GET)
=======
        // GET: Comercio
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Comercios.ToListAsync();
            return View(lista);
        }

        // GET: Comercio/Create
>>>>>>> 6ca8f85454cdf3d6862cb503bb68d15af25d95b6
        public IActionResult Create()
        {
            return View();
        }

<<<<<<< HEAD
        // 🔹 REGISTRAR (POST)
=======
        // POST: Comercio/Create
>>>>>>> 6ca8f85454cdf3d6862cb503bb68d15af25d95b6
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comercio comercio)
        {
<<<<<<< HEAD
            // Validar identificación única
            bool existe = await _context.Comercios
                .AnyAsync(c => c.Identificacion == comercio.Identificacion);

            if (existe)
            {
                ModelState.AddModelError("Identificacion", "Ya existe un comercio con esta identificación.");
                return View(comercio);
            }

=======
>>>>>>> 6ca8f85454cdf3d6862cb503bb68d15af25d95b6
            if (ModelState.IsValid)
            {
                comercio.FechaDeRegistro = DateTime.Now;
                comercio.Estado = true;

                _context.Add(comercio);
                await _context.SaveChangesAsync();
<<<<<<< HEAD
=======

>>>>>>> 6ca8f85454cdf3d6862cb503bb68d15af25d95b6
                return RedirectToAction(nameof(Index));
            }

            return View(comercio);
        }

<<<<<<< HEAD
        // 🔹 EDITAR (GET)
=======
        // GET: Comercio/Edit/5
>>>>>>> 6ca8f85454cdf3d6862cb503bb68d15af25d95b6
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var comercio = await _context.Comercios.FindAsync(id);
<<<<<<< HEAD
=======

>>>>>>> 6ca8f85454cdf3d6862cb503bb68d15af25d95b6
            if (comercio == null)
                return NotFound();

            return View(comercio);
        }

<<<<<<< HEAD
        // 🔹 EDITAR (POST)
=======
        // POST: Comercio/Edit
>>>>>>> 6ca8f85454cdf3d6862cb503bb68d15af25d95b6
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Comercio comercio)
        {
            if (id != comercio.IdComercio)
                return NotFound();

            if (ModelState.IsValid)
            {
                comercio.FechaDeModificacion = DateTime.Now;

                _context.Update(comercio);
                await _context.SaveChangesAsync();
<<<<<<< HEAD
=======

>>>>>>> 6ca8f85454cdf3d6862cb503bb68d15af25d95b6
                return RedirectToAction(nameof(Index));
            }

            return View(comercio);
        }
<<<<<<< HEAD
=======

        // GET: Comercio/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var comercio = await _context.Comercios
                .FirstOrDefaultAsync(m => m.IdComercio == id);

            if (comercio == null)
                return NotFound();

            return View(comercio);
        }

        // POST: Comercio/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comercio = await _context.Comercios.FindAsync(id);

            if (comercio == null)
                return NotFound();

            _context.Comercios.Remove(comercio);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
>>>>>>> 6ca8f85454cdf3d6862cb503bb68d15af25d95b6
    }
}