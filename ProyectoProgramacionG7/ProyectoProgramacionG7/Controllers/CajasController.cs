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

        // LISTAR
        public async Task<IActionResult> Index()
        {
            var cajas = await _context.Cajas
                .Include(c => c.Comercio)
                .Select(c => new Caja
                {
                    IdCaja = c.IdCaja,
                    IdComercio = c.IdComercio,
                    Comercio = c.Comercio,
                    Nombre = c.Nombre ?? "Sin Nombre",
                    Codigo = c.Codigo ?? "Sin Código",
                    Telefono = c.Telefono ?? "Sin Teléfono",
                    Estado = c.Estado,
                    FechaDeRegistro = c.FechaDeRegistro,
                    FechaDeModificacion = c.FechaDeModificacion,
                    UsuarioRegistro = c.UsuarioRegistro ?? "",
                    UsuarioModificacion = c.UsuarioModificacion ?? ""
                })
                .ToListAsync();

            return View(cajas);
        }

        // ABRIR FORMULARIO
        public IActionResult Create()
        {
            ViewBag.Comercios = _context.Comercios.ToList();
            return View();
        }

        // GUARDAR
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Caja caja)
        {
            try
            {
                caja.FechaDeRegistro = DateTime.Now;
                caja.Estado = true;

                _context.Cajas.Add(caja);
                await _context.SaveChangesAsync();

                var datos = JsonSerializer.Serialize(caja);

                await _bitacora.RegistrarEvento(
                    "Cajas",
                    "Crear",
                    "Se creó una nueva caja",
                    "",
                    null,
                    datos
                );

                return RedirectToAction("Index");
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

        // EDITAR
        public async Task<IActionResult> Edit(int id)
        {
            var caja = await _context.Cajas.FindAsync(id);

            if (caja == null)
                return NotFound();

            ViewBag.Comercios = _context.Comercios.ToList();
            return View(caja);
        }

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
                    var anterior = await _context.Cajas
                        .AsNoTracking()
                        .FirstOrDefaultAsync(x => x.IdCaja == id);

                    var datosAntes = JsonSerializer.Serialize(anterior);

                    caja.FechaDeModificacion = DateTime.Now;

                    _context.Update(caja);
                    await _context.SaveChangesAsync();

                    var datosDespues = JsonSerializer.Serialize(caja);

                    await _bitacora.RegistrarEvento(
                        "Cajas",
                        "Editar",
                        "Se editó una caja",
                        "",
                        datosAntes,
                        datosDespues
                    );

                    return RedirectToAction("Index");
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

        // ELIMINAR
        public async Task<IActionResult> Delete(int id)
        {
            var caja = await _context.Cajas
                .Include(c => c.Comercio)
                .FirstOrDefaultAsync(m => m.IdCaja == id);

            if (caja == null)
                return NotFound();

            return View(caja);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var caja = await _context.Cajas.FindAsync(id);

                if (caja == null)
                    return NotFound();

                var datosAntes = JsonSerializer.Serialize(caja);

                _context.Cajas.Remove(caja);
                await _context.SaveChangesAsync();

                await _bitacora.RegistrarEvento(
                    "Cajas",
                    "Eliminar",
                    "Se eliminó una caja",
                    "",
                    datosAntes,
                    null
                );

                return RedirectToAction("Index");
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

                return RedirectToAction("Index");
            }
        }
    }
}