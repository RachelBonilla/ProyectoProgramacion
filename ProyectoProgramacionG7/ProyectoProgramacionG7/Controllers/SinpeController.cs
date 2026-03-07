using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionG7.Data;
using Modelos.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProyectoProgramacionG7.Controllers
{
    public class SinpeController : Controller
    {
        private readonly AppDbContext _context;

        public SinpeController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Sinpe/Create
        public IActionResult Create()
        {
            ViewBag.Cajas = _context.Cajas.Where(c => c.Estado == true).ToList();
            return View();
        }

        // POST: Sinpe/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sinpe sinpe)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Cajas = _context.Cajas.Where(c => c.Estado == true).ToList();
                return View(sinpe);
            }

            var caja = await _context.Cajas.FirstOrDefaultAsync(c => c.IdCaja == sinpe.CajaId);

            if (caja == null || caja.Estado != true)
            {
                ModelState.AddModelError("", "Caja no válida o inactiva.");
                ViewBag.Cajas = _context.Cajas.Where(c => c.Estado == true).ToList();
                return View(sinpe);
            }

            sinpe.FechaDeRegistro = DateTime.Now;
            sinpe.Estado = false; // No sincronizado por defecto

            _context.Sinpes.Add(sinpe);
            await _context.SaveChangesAsync();

            ViewBag.Mensaje = "Pago SINPE registrado correctamente.";
            ViewBag.Cajas = _context.Cajas.Where(c => c.Estado == true).ToList();
            ModelState.Clear();

            return View();
        }

        // GET: Sinpe/Index
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