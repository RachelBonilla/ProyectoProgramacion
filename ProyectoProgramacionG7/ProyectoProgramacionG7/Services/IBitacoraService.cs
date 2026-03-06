using Modelos.Models;
using System.Threading.Tasks;

namespace ProyectoProgramacionG7.Services
{
    public interface IBitacoraService
    {
        Task RegistrarEventoAsync(string tablaDeEvento, string tipoDeEvento,
            string descripcion, string datosAnteriores = null,
            string datosPosteriores = null, string stackTrace = "");
    }
}