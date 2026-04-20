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
    public class SinpeController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacora;

        public SinpeController(AppDbContext context, IBitacoraService bitacora)
        {
            _context = context;
            _bitacora = bitacora;
        }

       
        public IActionResult Create()
        {
            ViewBag.Cajas = _context.Cajas.Where(c => c.Estado == true).ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sinpe sinpe)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Cajas = _context.Cajas.Where(c => c.Estado == true).ToList();
                    return View(sinpe);
                }

                var caja = await _context.Cajas
                    .FirstOrDefaultAsync(c => c.IdCaja == sinpe.CajaId);

                if (caja == null || !caja.Estado)
                {
                    ModelState.AddModelError("", "La caja no existe o está inactiva.");
                    ViewBag.Cajas = _context.Cajas.Where(c => c.Estado == true).ToList();
                    return View(sinpe);
                }

                if (caja.TelefonoSINPE != sinpe.TelefonoDestinatario)
                {
                    ModelState.AddModelError("", "El teléfono destinatario no coincide con la caja.");
                    ViewBag.Cajas = _context.Cajas.Where(c => c.Estado == true).ToList();
                    return View(sinpe);
                }

                sinpe.FechaDeRegistro = DateTime.Now;
                sinpe.Estado = false; // NO sincronizado

                _context.Sinpes.Add(sinpe);
                await _context.SaveChangesAsync();

                var datos = JsonSerializer.Serialize(sinpe);

                await _bitacora.RegistrarEvento(
                    "SINPE",
                    "Registrar",
                    "Se registró un pago SINPE",
                    "",
                    datos,
                    null
                );

                ViewBag.Mensaje = "Pago registrado correctamente.";
                ModelState.Clear();
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "SINPE",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );
            }

            ViewBag.Cajas = _context.Cajas.Where(c => c.Estado == true).ToList();
            return View();
        }

        public async Task<IActionResult> Index()
        {
            var sinpes = await _context.Sinpes
                .Include(s => s.Caja)
                .ThenInclude(c => c.Comercio)
                .ToListAsync();

            return View(sinpes);
        }
    }
}