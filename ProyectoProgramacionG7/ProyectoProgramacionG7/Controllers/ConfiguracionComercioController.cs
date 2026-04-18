using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Models;
using ProyectoProgramacionG7.Data;
using ProyectoProgramacionG7.Services;
using System.Text.Json;

namespace ProyectoProgramacionG7.Controllers
{
    [Authorize]
    public class ConfiguracionComercioController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacora;

        public ConfiguracionComercioController(AppDbContext context, IBitacoraService bitacora)
        {
            _context = context;
            _bitacora = bitacora;
        }
        public async Task<IActionResult> Index()
        {
            var lista = await _context.Configuraciones
                .Include(c => c.Comercio)
                .ToListAsync();

            return View(lista);
        }

  
        public IActionResult Create()
        {
            ViewBag.Comercios = _context.Comercios.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ConfiguracionComercio config)
        {
            try
            {
                bool existe = await _context.Configuraciones
                    .AnyAsync(c => c.IdComercio == config.IdComercio);

                if (existe)
                {
                    ModelState.AddModelError("", "Ya existe una configuración para este comercio.");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Comercios = _context.Comercios.ToList();
                    return View(config);
                }

                config.FechaDeRegistro = DateTime.Now;
                config.Estado = true;

                _context.Configuraciones.Add(config);
                await _context.SaveChangesAsync();

                var datos = JsonSerializer.Serialize(config);

                await _bitacora.RegistrarEvento(
                    "Configuracion",
                    "Registrar",
                    "Se creó configuración",
                    "",
                    datos,
                    null
                );

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Configuracion",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );
            }

            ViewBag.Comercios = _context.Comercios.ToList();
            return View(config);
        }

      
        public async Task<IActionResult> Edit(int id)
        {
            var config = await _context.Configuraciones.FindAsync(id);

            if (config == null)
                return NotFound();

            ViewBag.Comercios = _context.Comercios.ToList();
            return View(config);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ConfiguracionComercio config)
        {
            if (id != config.IdConfiguracion)
                return NotFound();

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Comercios = _context.Comercios.ToList();
                    return View(config);
                }

                var anterior = await _context.Configuraciones
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.IdConfiguracion == id);

                var datosAntes = JsonSerializer.Serialize(anterior);

                config.FechaDeModificacion = DateTime.Now;

                _context.Update(config);
                await _context.SaveChangesAsync();

                var datosDespues = JsonSerializer.Serialize(config);

                await _bitacora.RegistrarEvento(
                    "Configuracion",
                    "Editar",
                    "Se editó configuración",
                    "",
                    datosAntes,
                    datosDespues
                );

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Configuracion",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );
            }

            ViewBag.Comercios = _context.Comercios.ToList();
            return View(config);
        }
    }
}