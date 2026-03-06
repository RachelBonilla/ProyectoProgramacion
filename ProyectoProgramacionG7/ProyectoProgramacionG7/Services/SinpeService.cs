using Modelos.Models;
using Microsoft.EntityFrameworkCore;
using ProyectoProgramacionG7.Data;
using System;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProyectoProgramacionG7.Services
{
    public class SinpeService : ISinpeService
    {
        private readonly AppDbContext _context;
        private readonly IBitacoraService _bitacoraService;

        public SinpeService(AppDbContext context, IBitacoraService bitacoraService)
        {
            _context = context;
            _bitacoraService = bitacoraService;
        }

        public async Task<bool> ExisteTelefonoActivoAsync(string telefonoDestinatario)
        {
            return await _context.Cajas
                .AnyAsync(c => c.TelefonoSINPE == telefonoDestinatario && c.Estado == true);
        }

        public async Task<bool> RegistrarPagoAsync(Sinpe sinpe)
        {
            try
            {
                sinpe.FechaDeRegistro = DateTime.Now;
                sinpe.Estado = false; 

                _context.Sinpes.Add(sinpe);
                await _context.SaveChangesAsync();

                var datosJson = JsonSerializer.Serialize(sinpe);
                await _bitacoraService.RegistrarEventoAsync(
                    tablaDeEvento: "Sinpe",
                    tipoDeEvento: "Registrar",
                    descripcion: "Se registró un nuevo pago SINPE",
                    datosAnteriores: datosJson
                );

                return true;
            }
            catch (Exception ex)
            {
                await _bitacoraService.RegistrarEventoAsync(
                    tablaDeEvento: "Sinpe",
                    tipoDeEvento: "Error",
                    descripcion: ex.Message,
                    stackTrace: ex.StackTrace
                );

                return false;
            }
        }
    }
}