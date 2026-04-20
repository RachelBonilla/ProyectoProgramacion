using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Models;
using ProyectoProgramacionG7.Data;

namespace ProyectoProgramacionG7.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ReportesController : Controller
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var reportes = await _context.Reportes
                .Include(r => r.Comercio)
                .OrderByDescending(r => r.FechaDelReporte)
                .ToListAsync();

            return View(reportes);
        }

        public async Task<IActionResult> Generar()
        {
            var comercios = await _context.Comercios.ToListAsync();

            foreach (var comercio in comercios)
            {
                var cajas = await _context.Cajas
                    .Where(c => c.IdComercio == comercio.IdComercio)
                    .ToListAsync();

                int cantidadCajas = cajas.Count;

                var cajaIds = cajas.Select(c => c.IdCaja).ToList();
                var sinpes = await _context.Sinpes
                    .Include(s => s.Caja)
                    .Where(s => s.Caja != null && cajaIds.Contains(s.Caja.IdCaja))
                    .ToListAsync();

                decimal montoTotal = sinpes.Sum(s => s.Monto);
                int cantidadSinpes = sinpes.Count;

                var config = await _context.Configuraciones
                    .FirstOrDefaultAsync(c => c.IdComercio == comercio.IdComercio);

                decimal comision = 0;

                if (config != null)
                {
                    comision = montoTotal * (config.Comision / 100m);
                }

                var fecha = DateTime.Now;

                var existente = await _context.Reportes
                    .FirstOrDefaultAsync(r =>
                        r.IdComercio == comercio.IdComercio &&
                        r.FechaDelReporte.Month == fecha.Month &&
                        r.FechaDelReporte.Year == fecha.Year);

                if (existente != null)
                {
                    existente.CantidadDeCajas = cantidadCajas;
                    existente.MontoTotalRecaudado = montoTotal;
                    existente.CantidadDeSINPES = cantidadSinpes;
                    existente.MontoTotalComision = comision;
                }
                else
                {
                    var nuevo = new ReporteMensual
                    {
                        IdComercio = comercio.IdComercio,
                        CantidadDeCajas = cantidadCajas,
                        MontoTotalRecaudado = montoTotal,
                        CantidadDeSINPES = cantidadSinpes,
                        MontoTotalComision = comision,
                        FechaDelReporte = fecha
                    };

                    _context.Reportes.Add(nuevo);
                }
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}