using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionG7.Data;
using Modelos.Models;
using ProyectoProgramacionG7.Services;
using System.Text.Json;

namespace ProyectoProgramacionG7.Controllers
{
    public class CajasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacora;

        public CajasController(AppDbContext context, IBitacoraService bitacora)
        {
            _context = context;
            _bitacora = bitacora;
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
            try
            {
                if (ModelState.IsValid)
                {
                    caja.FechaDeRegistro = DateTime.Now;
                    caja.Estado = true;

                    _context.Add(caja);
                    await _context.SaveChangesAsync();

                    var datos = JsonSerializer.Serialize(caja);

                    await _bitacora.RegistrarEvento(
                        "Cajas",
                        "Registrar",
                        "Se registró una nueva caja",
                        "",
                        datos,
                        null
                    );

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Cajas",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );
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

            try
            {
                if (ModelState.IsValid)
                {
                    var cajaAnterior = await _context.Cajas
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.IdCaja == id);

                    var datosAnteriores = JsonSerializer.Serialize(cajaAnterior);

                    caja.FechaDeModificacion = DateTime.Now;

                    _context.Update(caja);
                    await _context.SaveChangesAsync();

                    var datosPosteriores = JsonSerializer.Serialize(caja);

                    await _bitacora.RegistrarEvento(
                        "Cajas",
                        "Editar",
                        "Se editó una caja",
                        "",
                        datosAnteriores,
                        datosPosteriores
                    );

                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Cajas",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );
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
            try
            {
                var caja = await _context.Cajas.FindAsync(id);

                if (caja == null)
                    return NotFound();

                var datosAnteriores = JsonSerializer.Serialize(caja);

                _context.Cajas.Remove(caja);
                await _context.SaveChangesAsync();

                await _bitacora.RegistrarEvento(
                    "Cajas",
                    "Eliminar",
                    "Se eliminó una caja",
                    "",
                    datosAnteriores,
                    null
                );

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Cajas",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );

                return RedirectToAction(nameof(Index));
            }
        }
    }
}