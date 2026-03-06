using Modelos.Models;
using ProyectoProgramacionG7.Data;
using System.Text.Json;

namespace ProyectoProgramacionG7.Services
{
    public class BitacoraService : IBitacoraService
    {
        private readonly AppDbContext _context;

        public BitacoraService(AppDbContext context)
        {
            _context = context;
        }

        public async Task RegistrarEvento(
            string tabla,
            string tipoEvento,
            string descripcion,
            string stackTrace,
            string datosAnteriores,
            string datosPosteriores)
        {
            var evento = new BitacoraEvento
            {
                TablaDeEvento = tabla,
                TipoDeEvento = tipoEvento,
                FechaDeEvento = DateTime.Now,
                DescripcionDeEvento = descripcion,
                StackTrace = stackTrace,
                DatosAnteriores = datosAnteriores,
                DatosPosteriores = datosPosteriores
            };

            _context.BitacoraEventos.Add(evento);
            await _context.SaveChangesAsync();
        }
    }
}