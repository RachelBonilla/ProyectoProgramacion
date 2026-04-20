using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Models;
using ProyectoProgramacionG7.Data;
using ProyectoProgramacionG7.Services;
using System.Text.Json;

namespace ProyectoProgramacionG7.Controllers
{

    [Authorize(Roles = "Administrador")]
    public class ComercioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacora;

        public ComercioController(AppDbContext context, IBitacoraService bitacora)
        {
            _context = context;
            _bitacora = bitacora;
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
            try
            {
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

                    var datos = JsonSerializer.Serialize(comercio);

                    await _bitacora.RegistrarEvento(
                        "Comercios",
                        "Registrar",
                        "Se registró un comercio",
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
                    "Comercios",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );
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

            try
            {
                if (ModelState.IsValid)
                {
                    var comercioAnterior = await _context.Comercios
                        .AsNoTracking()
                        .FirstOrDefaultAsync(c => c.IdComercio == id);

                    var datosAnteriores = JsonSerializer.Serialize(comercioAnterior);

                    comercio.FechaDeModificacion = DateTime.Now;

                    _context.Comercios.Update(comercio);
                    await _context.SaveChangesAsync();

                    var datosPosteriores = JsonSerializer.Serialize(comercio);

                    await _bitacora.RegistrarEvento(
                        "Comercios",
                        "Editar",
                        "Se editó un comercio",
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
                    "Comercios",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );
            }

            return View(comercio);
        }
    }
}