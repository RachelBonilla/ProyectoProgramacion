using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionG7.Data;
using Modelos.Models;

namespace ProyectoProgramacionG7.Controllers
{
    public class CajasController : Controller
    {
        private readonly AppDbContext _context;

        public CajasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Cajas
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Cajas
                .Include(c => c.Comercio)
                .ToListAsync();

            return View(lista);
        }

        // GET: Cajas/Create
        public IActionResult Create()
        {
            ViewBag.Comercios = _context.Comercios.ToList();
            return View();
        }

        // POST: Cajas/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Caja caja)
        {
            if (ModelState.IsValid)
            {
                caja.FechaDeRegistro = DateTime.Now;
                caja.Estado = true;

                _context.Add(caja);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Comercios = _context.Comercios.ToList();
            return View(caja);
        }

        // GET: Cajas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var caja = await _context.Cajas.FindAsync(id);

            if (caja == null)
                return NotFound();

            ViewBag.Comercios = _context.Comercios.ToList();
            return View(caja);
        }

        // POST: Cajas/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Caja caja)
        {
            if (id != caja.IdCaja)
                return NotFound();

            if (ModelState.IsValid)
            {
                caja.FechaDeModificacion = DateTime.Now;

                _context.Update(caja);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewBag.Comercios = _context.Comercios.ToList();
            return View(caja);
        }

        // GET: Cajas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var caja = await _context.Cajas
                .Include(c => c.Comercio)
                .FirstOrDefaultAsync(m => m.IdCaja == id);

            if (caja == null)
                return NotFound();

            return View(caja);
        }

        // POST: Cajas/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var caja = await _context.Cajas.FindAsync(id);

            if (caja == null)
                return NotFound();

            _context.Cajas.Remove(caja);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}