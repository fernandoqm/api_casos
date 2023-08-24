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
using System;
using Microsoft.Identity.Client;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cors;

namespace api_casos.Controllers
{
    [Route("api/seguridad")]
    [EnableCors("ReglasCors")]
    [ApiController]
    public class TokenController : ControllerBase
    {

        public IConfiguration _configuration;
        private readonly DataBaseContext _context;
        private readonly string secretKey;
        private readonly JWTSetting jWT;
        private readonly IRefreshTokenGenerator tokenGenerador;


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
                        expires: DateTime.UtcNow.AddSeconds(30),
                        signingCredentials: sigIn
                        );


                    var tokenHandler = new JwtSecurityTokenHandler();
                    string tokencreado = tokenHandler.WriteToken(token);

                    return StatusCode(StatusCodes.Status200OK, new { token = tokencreado });

                }
                else
                {
                    return StatusCode(StatusCodes.Status409Conflict, new { mensaje = "Error al validar el usuario", token = "" });
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

        [NonAction]
        public TokenResponse Validar(string username, Claim[] claims)
        {
            TokenResponse tokenResponse = new TokenResponse();
            var tokenkey = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);
            var tokenhandler = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddMinutes(15),
                 signingCredentials: new SigningCredentials(new SymmetricSecurityKey(tokenkey), SecurityAlgorithms.HmacSha256)

                );
            tokenResponse.JWTToken = new JwtSecurityTokenHandler().WriteToken(tokenhandler);
            tokenResponse.RefreshToken = tokenGenerador.GenerateToken(username);

            return tokenResponse;
        }

    }
}
