using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Modelos.Models;
using ProyectoProgramacionG7.Data;
using ProyectoProgramacionG7.Services;
using System.Text.Json;

namespace ProyectoProgramacionG7.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacora;

        public UsuariosController(AppDbContext context, IBitacoraService bitacora)
        {
            _context = context;
            _bitacora = bitacora;
        }

        public async Task<IActionResult> Index()
        {
            var lista = await _context.Usuarios
                .Include(u => u.Comercio)
                .ToListAsync();

            return View(lista);
        }

        public IActionResult Create()
        {
            ViewBag.Comercios = new SelectList(_context.Comercios, "IdComercio", "Nombre");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            try
            {
                bool existe = await _context.Usuarios
                    .AnyAsync(u => u.Identificacion == usuario.Identificacion);

                if (existe)
                {
                    ModelState.AddModelError("", "Ya existe un usuario con esa identificación.");
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Comercios = new SelectList(_context.Comercios, "IdComercio", "Nombre");
                    return View(usuario);
                }

                usuario.FechaDeRegistro = DateTime.Now;
                usuario.Estado = true;

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                var datos = JsonSerializer.Serialize(usuario);

                await _bitacora.RegistrarEvento(
                    "Usuarios",
                    "Registrar",
                    "Se creó usuario",
                    "",
                    datos,
                    null
                );

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Usuarios",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );
            }

            ViewBag.Comercios = new SelectList(_context.Comercios, "IdComercio", "Nombre");
            return View(usuario);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);

            if (usuario == null)
                return NotFound();

            ViewBag.Comercios = new SelectList(_context.Comercios, "IdComercio", "Nombre");

            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Usuario usuario)
        {
            if (id != usuario.IdUsuario)
                return NotFound();

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewBag.Comercios = new SelectList(_context.Comercios, "IdComercio", "Nombre", usuario.IdComercio);
                    return View(usuario);
                }

                var anterior = await _context.Usuarios
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.IdUsuario == id);

                var datosAntes = JsonSerializer.Serialize(anterior);

                usuario.FechaDeModificacion = DateTime.Now;

                _context.Update(usuario);
                await _context.SaveChangesAsync();

                var datosDespues = JsonSerializer.Serialize(usuario);

                await _bitacora.RegistrarEvento(
                    "Usuarios",
                    "Editar",
                    "Se editó usuario",
                    "",
                    datosAntes,
                    datosDespues
                );

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                await _bitacora.RegistrarEvento(
                    "Usuarios",
                    "Error",
                    ex.Message,
                    ex.StackTrace,
                    null,
                    null
                );
            }

            ViewBag.Comercios = new SelectList(_context.Comercios, "IdComercio", "Nombre", usuario.IdComercio);
            return View(usuario);
        }
    }
}