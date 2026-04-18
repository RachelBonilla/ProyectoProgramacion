using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProyectoProgramacionG7.Data;
using ProyectoProgramacionG7.ViewModels;

namespace ProyectoProgramacionG7.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _context;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var resultado = await _signInManager.PasswordSignInAsync(
                model.Correo,
                model.Contrasena,
                isPersistent: false,
                lockoutOnFailure: false
            );

            if (resultado.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Correo o contraseña incorrectos.");
            return View(model);
        }

        // GET: /Account/Registro
        public IActionResult Registro()
        {
            return View();
        }

        // POST: /Account/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Si es Cajero, verificar que existe en G7_Usuarios
            if (model.Rol == "Cajero")
            {
                var usuarioExiste = _context.Usuarios
                    .Any(u => u.CorreoElectronico == model.Correo && u.Estado == true);

                if (!usuarioExiste)
                {
                    ModelState.AddModelError("", "El cajero debe existir previamente en la tabla de usuarios.");
                    return View(model);
                }
            }

            var user = new IdentityUser
            {
                UserName = model.Correo,
                Email = model.Correo
            };

            var resultado = await _userManager.CreateAsync(user, model.Contrasena);

            if (resultado.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, model.Rol);

                // Si es Cajero, actualizar IdNetUser en G7_Usuarios
                if (model.Rol == "Cajero")
                {
                    var usuario = _context.Usuarios
                        .FirstOrDefault(u => u.CorreoElectronico == model.Correo);

                    if (usuario != null)
                    {
                        usuario.IdNetUser = Guid.Parse(user.Id);
                        await _context.SaveChangesAsync();
                    }
                }

                return RedirectToAction("Login");
            }

            foreach (var error in resultado.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }

        // POST: /Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}