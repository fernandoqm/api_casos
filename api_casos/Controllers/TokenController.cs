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
    [Route("api/token")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        public IConfiguration _configuration;
        private readonly DataBaseContext _context;

        public TokenController(IConfiguration config, DataBaseContext context)
        {
            _configuration = config;    
            _context = context;
        }


        [HttpPost]
        public async Task<IActionResult> Post(Usuarios _usuarios)
        {
            if (_usuarios != null && _usuarios.usuario != null && _usuarios.clave != null) 
            {
                var user = await GetUser(_usuarios.usuario, _usuarios.clave);

                if(user != null)
                {
                    var claims = new[]
                    {
                        new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, _configuration["Jwt:Key"]),
                        new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("id_usuario", user.id_usuario.ToString()),
                        new Claim("nombre", user.nombre),
                        new Claim("usuario", user.usuario),
                        new Claim("correo", user.correo)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        //expires: DateTime.UtcNow.AddMinutes(10),
                        expires: DateTime.UtcNow.AddDays(1),
                        signingCredentials: signIn);

                    return Ok(new JwtSecurityTokenHandler().WriteToken(token));
                }
                else
                {
                    return BadRequest("Datos incorrectos");
                }
            }
            else
            {
                return BadRequest();
            }
        }

        private async Task<Usuarios> GetUser(string pUsuario, string pClave)
        {
            return await _context.UsuarioInfo.FirstOrDefaultAsync(usua => usua.usuario == pUsuario && usua.clave == pClave);
        }



    }
}
