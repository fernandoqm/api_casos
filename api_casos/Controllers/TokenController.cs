using api_casos.Datos;
using api_casos.Datos.Interfaces;
using api_casos.Modelos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.JsonWebTokens;

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

                if (user != null)
                {

                    var claims = new[]
                    {
                        new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, _configuration["Jwt:Subject"]),
                        new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim("Id_usuario", user.id_usuario.ToString()),
                        new Claim("Nombre", (user.nombre + "" +user.apellido1+""+user.apellido2)),
                        new Claim("Usuario", user.usuario),
                        new Claim("Correo",user.correo)
                    };

                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
                    var sigIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                    var token = new JwtSecurityToken(
                        _configuration["Jwt:Issuer"],
                        _configuration["Jwt:Audience"],
                        claims,
                        expires: DateTime.UtcNow.AddHours(5),
                        signingCredentials: sigIn
                        );


                    var tokenHandler = new JwtSecurityTokenHandler();
                    string tokencreado = tokenHandler.WriteToken(token);

                    return StatusCode(StatusCodes.Status200OK, new { token = tokencreado });

                }
                else
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { mensaje = "Error al crear el token", token = "" });
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
