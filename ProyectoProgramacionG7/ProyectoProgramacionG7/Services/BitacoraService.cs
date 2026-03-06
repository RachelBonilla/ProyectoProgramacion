using Modelos.Models;
using ProyectoProgramacionG7.Data;
using System;
using System.Threading.Tasks;

namespace ProyectoProgramacionG7.Services
{
    public class BitacoraService : IBitacoraService
    {
        private readonly AppDbContext _context;

        public BitacoraService(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarEventoAsync(string tablaDeEvento, string tipoDeEvento,
            string descripcion, string datosAnteriores = null,
            string datosPosteriores = null, string stackTrace = "")
        {
            var evento = new BitacoraEvento
            {
                TablaDeEvento = tablaDeEvento,
                TipoDeEvento = tipoDeEvento,
                FechaDeEvento = DateTime.Now,
                DescripcionDeEvento = descripcion,
                StackTrace = stackTrace ?? "",
                DatosAnteriores = datosAnteriores,
                DatosPosteriores = datosPosteriores
            };

            _context.BitacoraEventos.Add(evento);
            await _context.SaveChangesAsync();
        }
    }
}