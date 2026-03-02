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

        // GET: Comercio
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Comercios.ToListAsync();
            return View(lista);
        }

        // GET: Comercio/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Comercio/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Comercio comercio)
        {
            if (ModelState.IsValid)
            {
                comercio.FechaDeRegistro = DateTime.Now;
                comercio.Estado = true;

                _context.Add(comercio);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(comercio);
        }

        // GET: Comercio/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var comercio = await _context.Comercios.FindAsync(id);

            if (comercio == null)
                return NotFound();

            return View(comercio);
        }

        // POST: Comercio/Edit
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

                return RedirectToAction(nameof(Index));
            }

            return View(comercio);
        }

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
    }
}