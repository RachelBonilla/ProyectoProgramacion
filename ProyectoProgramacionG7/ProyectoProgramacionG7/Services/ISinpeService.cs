using Modelos.Models;
using System.Threading.Tasks;

namespace ProyectoProgramacionG7.Services
{
    public interface ISinpeService
    {
        Task<bool> RegistrarPagoAsync(Sinpe sinpe);
        Task<bool> ExisteTelefonoActivoAsync(string telefonoDestinatario);
    }
}