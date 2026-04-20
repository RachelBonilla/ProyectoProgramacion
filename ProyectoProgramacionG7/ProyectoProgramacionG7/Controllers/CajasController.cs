using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Models;
using ProyectoProgramacionG7.Data;
using ProyectoProgramacionG7.Services;
using System.Text.Json;

namespace ProyectoProgramacionG7.Controllers
{
    [Authorize(Roles = "Administrador,Cajero")]
    public class CajasController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacora;
        private readonly UserManager<IdentityUser> _userManager;

        public CajasController(AppDbContext context, IBitacoraService bitacora, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _bitacora = bitacora;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int? comercioId)
        {
            var usuario = await _userManager.GetUserAsync(User);
            var roles = await _userManager.GetRolesAsync(usuario!);
            var esCajero = roles.Contains("Cajero");

            var cajas = _context.Cajas
                .Include(c => c.Comercio)
                .AsQueryable();

            if (esCajero)
            {
                var usuarioG7 = _context.Usuarios
                    .FirstOrDefault(u => u.CorreoElectronico == usuario!.Email);

                if (usuarioG7 == null)
                    return View(new List<Caja>());

                cajas = cajas.Where(c => c.IdComercio == usuarioG7.IdComercio);
            }
            else if (comercioId != null)
            {
                cajas = cajas.Where(c => c.IdComercio == comercioId);
            }

            var lista = await cajas.ToListAsync();

            foreach (var c in lista)
            {
                c.Nombre ??= "Sin nombre";
                c.Descripcion ??= "Sin descripción";
                c.TelefonoSINPE ??= "Sin teléfono";
            }

            return View(lista);
        }

        public IActionResult Create()
        {
            ViewBag.Comercios = _context.Comercios.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Caja caja)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Comercios = _context.Comercios.ToList();
                    return View(caja);
                }

                caja.Nombre ??= "Sin nombre";
                caja.Descripcion ??= "Sin descripción";
                caja.TelefonoSINPE ??= "Sin teléfono";

                caja.FechaDeRegistro = DateTime.Now;
                caja.Estado = true;

                _context.Cajas.Add(caja);
                await _context.SaveChangesAsync();

                var datos = JsonSerializer.Serialize(caja);

                await _bitacora.RegistrarEvento(
                    "Cajas",
                    "Registrar",
                    "Se registró una caja",
                    "",
                    datos,
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
            }

            ViewBag.Comercios = _context.Comercios.ToList();
            return View(caja);
        }

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
                if (!ModelState.IsValid)
                {
                    ViewBag.Comercios = _context.Comercios.ToList();
                    return View(caja);
                }

                caja.Nombre ??= "Sin nombre";
                caja.Descripcion ??= "Sin descripción";
                caja.TelefonoSINPE ??= "Sin teléfono";

                caja.FechaDeModificacion = DateTime.Now;

                _context.Update(caja);
                await _context.SaveChangesAsync();

                var datos = JsonSerializer.Serialize(caja);

                await _bitacora.RegistrarEvento(
                    "Cajas",
                    "Editar",
                    "Se editó una caja",
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

        public async Task<IActionResult> VerSinpe(int id)
        {
            var sinpes = await _context.Sinpes
                .Where(s => s.CajaId.HasValue && s.CajaId == id)
                .OrderByDescending(s => s.FechaDeRegistro)
                .ToListAsync();

            return View(sinpes);
        }

        public async Task<IActionResult> Sincronizar(int id)
        {
            try
            {
                var sinpe = await _context.Sinpes.FindAsync(id);

                if (sinpe == null)
                    return NotFound();

                if (sinpe.Estado == true)
                    return RedirectToAction("VerSinpe", new { id = sinpe.CajaId });

                sinpe.Estado = true;

                await _context.SaveChangesAsync();

                return RedirectToAction("VerSinpe", new { id = sinpe.CajaId });
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }
    }
}