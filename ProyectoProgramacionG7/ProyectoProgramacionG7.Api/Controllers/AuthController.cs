using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProyectoProgramacionG7.Api.Data;
using ProyectoProgramacionG7.Api.DataTransfer;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ProyectoProgramacionG7.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST: api/Auth/Token
        [HttpPost("Token")]
        public async Task<ActionResult<TokenResponse>> ObtenerToken([FromBody] int idComercio)
        {
            // verificar que el comercio existe
            var comercio = await _context.Comercios
                .FirstOrDefaultAsync(c => c.IdComercio == idComercio);

            if (comercio == null)
            {
                return Unauthorized(new TokenResponse
                {
                    EsValido = false,
                    Mensaje = "El comercio no existe.",
                    Token = null
                });
            }

            //  Verificar configuración 
            var config = await _context.Configuraciones
                .FirstOrDefaultAsync(c => c.IdComercio == idComercio);

            if (config == null || (config.TipoConfiguracion != 2 && config.TipoConfiguracion != 3))
            {
                return Unauthorized(new TokenResponse
                {
                    EsValido = false,
                    Mensaje = "Este comercio no esta habilitado para sincronizacion externa",
                    Token = null
                });
            }

            // 3. generar el token
            var token = GenerarToken(idComercio, comercio.Nombre);

            return Ok(new TokenResponse
            {
                EsValido = true,
                Mensaje = "Token generado correctamente.",
                Token = token
            });
        }

        private string GenerarToken(int idComercio, string nombreComercio)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );

            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim("idComercio", idComercio.ToString()),
                new Claim("nombreComercio", nombreComercio)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credenciales
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}