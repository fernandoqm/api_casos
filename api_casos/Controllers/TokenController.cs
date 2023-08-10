using api_casos.Datos;
using api_casos.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_casos.Controllers
{
    [Route("api/seguridad")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        public IConfiguration _configuration;
        private readonly DataBaseContext _context;
        private readonly string secretKey;

        public TokenController(IConfiguration config, DataBaseContext context)
        {
            secretKey = config.GetSection("Jwt").GetSection("key").ToString();
            _configuration = config;    
            _context = context;
        }


        [HttpPost]
        [Route("Validar")]
        public async Task<IActionResult> Validar([FromBody]Usuarios _usuarios)
        {
            if (_usuarios != null && _usuarios.usuario != null && _usuarios.clave != null) 
            {
                var user = await GetUser(_usuarios.usuario, _usuarios.clave);

                if(user != null)
                {
                    var keyBytes = Encoding.ASCII.GetBytes(secretKey);
                    ClaimsPrincipal principal = new ClaimsPrincipal();
                    var claims = (ClaimsIdentity)principal.Identity;

                    claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, _usuarios.usuario));
                    claims.AddClaim(new Claim(ClaimTypes.Email, _usuarios.correo));
                    claims.AddClaim(new Claim(ClaimTypes.Name, (_usuarios.nombre +" "+_usuarios.apellido1+" "+_usuarios.apellido2)));

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddMinutes(5),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                    string tokencreado = tokenHandler.WriteToken(tokenConfig);

                    return StatusCode(StatusCodes.Status200OK, new { token = tokencreado });

                }
                else
                {
                    return StatusCode(StatusCodes.Status409Conflict, new {mensaje = "Error al crear el token", token = "" });
                }
            }
            else
            {
                return StatusCode(StatusCodes.Status404NotFound, new {mensaje = "Sin datos de usuario", token = "" });
            }
        }

        private async Task<Usuarios> GetUser(string pUsuario, string pClave)
        {
            return await _context.UsuarioInfo.FirstOrDefaultAsync(usua => usua.usuario == pUsuario && usua.clave == pClave);
        }



    }
}
