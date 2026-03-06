using Microsoft.AspNetCore.Mvc;
using Modelos.Models;
using ProyectoProgramacionG7.Services;

using Modelos.Models;
using Microsoft.AspNetCore.Mvc;
using ProyectoProgramacionG7.Services;
using System.Threading.Tasks;

namespace ProyectoProgramacionG7.Controllers
{
    public class SinpeController : Controller
    {
        private readonly ISinpeService _sinpeService;

        public SinpeController(ISinpeService sinpeService)
        {
            _sinpeService = sinpeService;
        }

        
        public IActionResult Create()
        {
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Sinpe sinpe)
        {
            if (!ModelState.IsValid)
            {
                return View(sinpe);
            }

            bool telefonoExiste = await _sinpeService.ExisteTelefonoActivoAsync(sinpe.TelefonoDestinatario);
            if (!telefonoExiste)
            {
                ModelState.AddModelError("TelefonoDestinatario",
                    "El teléfono destinatario no está registrado o la caja se encuentra inactiva.");
                return View(sinpe);
            }

            bool resultado = await _sinpeService.RegistrarPagoAsync(sinpe);
            if (!resultado)
            {
                ModelState.AddModelError("", "Ocurrió un error al registrar el pago. Intente de nuevo.");
                return View(sinpe);
            }

            TempData["Exito"] = "Pago SINPE registrado exitosamente.";
            return RedirectToAction(nameof(Create));
        }
    }
}