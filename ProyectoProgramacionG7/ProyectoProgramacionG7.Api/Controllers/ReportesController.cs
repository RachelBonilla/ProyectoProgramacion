using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Modelos.Models;
using ProyectoProgramacionG7.Api.Data;

namespace ProyectoProgramacionG7.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReportesController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Reportes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReporteMensual>>> GetReportes()
        {
            return await _context.Reportes
                .Include(r => r.Comercio)
                .OrderByDescending(r => r.FechaDelReporte)
                .ToListAsync();
        }

        // GET: api/Reportes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReporteMensual>> GetReporte(int id)
        {
            var reporte = await _context.Reportes
                .Include(r => r.Comercio)
                .FirstOrDefaultAsync(r => r.IdReporte == id);

            if (reporte == null)
                return NotFound();

            return reporte;
        }

        // POST: api/Reportes/Generar
        [HttpPost("Generar")]
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
                    comision = montoTotal * (config.Comision / 100m);

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

            return Ok(new { mensaje = "Reportes generados correctamente." });
        }
    }
}