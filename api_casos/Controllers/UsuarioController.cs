using api_casos.Datos.Interfaces;
using api_casos.Modelos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_casos.Controllers
{
    [EnableCors("ReglasCors")]
    [Authorize]
    [Route("api/usuarios")]
    [ApiController]
    public class UsuarioController : ControllerBase
    {

        private readonly IUsuario _IUsuario;

        public UsuarioController(IUsuario _usuario)
        {
            _IUsuario = _usuario;
        }

        private bool UsuarioExiste(int id)
        {
            return _IUsuario.ValidaUsuario(id);
        }

        [HttpGet]
        [Route("ListaUsuarios")]
        public async Task<IActionResult> ListaUsuarios()
        {
            List<Usuarios> lista = new List<Usuarios>();
            try
            {
                lista = await Task.FromResult(_IUsuario.GetUsuarioDetails());
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "Ok", response = lista });
            }
            catch(Exception error)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { mensaje = error.Message + "Error de algo", response = lista });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Usuarios>> Get(int id)
        {
            var usuarios = await Task.FromResult(_IUsuario.GetUsuarioDetails(id));
            if(usuarios == null)
            {
                return NotFound();
            }
            return Ok(usuarios);
        }

        [HttpPost]
        public async Task<ActionResult<Usuarios>> Post(Usuarios usuario)
        {
            _IUsuario.AgregaUsuario(usuario);
            return await Task.FromResult(usuario);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Usuarios>> Put(int id, Usuarios usuario)
        {
            if(id != usuario.id_usuario)
            {
                return BadRequest();
            }
            try
            {
                _IUsuario.ActualizaUsuario(usuario);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExiste(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return await Task.FromResult(usuario);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Usuarios>> Delete(int id)
        {
            var usuario = _IUsuario.BorrarUsuario(id);
            return await Task.FromResult(usuario);
        }



    }
}
