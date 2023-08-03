using api_casos.Datos.Interfaces;
using api_casos.Modelos;
using Microsoft.EntityFrameworkCore;

namespace api_casos.Datos.Repositorios
{
    public class UsuarioRepositorio : IUsuario
    {
        readonly DataBaseContext _dbContext = new();

        public UsuarioRepositorio(DataBaseContext context)
        {
            _dbContext = context;
        }

        public void ActualizaUsuario(Usuarios usuario)
        {
            try
            {
                _dbContext.Entry(usuario).State = EntityState.Modified;
                _dbContext.SaveChanges();
            }
            catch
            {
                throw;
            }

        }

        public void AgregaUsuario(Usuarios usuario)
        {
            try
            {
                _dbContext.UsuarioInfo.Add(usuario);
                _dbContext.SaveChanges();
            }
            catch 
            {
                throw;
            }
        }

        public Usuarios BorrarUsuario(int id)
        {
            try
            {
                Usuarios? usuario = _dbContext.UsuarioInfo.Find(id);

                if(usuario != null)
                {
                    _dbContext.UsuarioInfo.Remove(usuario);
                    _dbContext.SaveChanges();
                    return usuario;
                }
                else
                {
                    throw new ArgumentNullException();
                }

            }
            catch (Exception ex)
            {
                throw ;
            }
            
        }

        public List<Usuarios> GetUsuarioDetails()
        {
            try
            {
                return _dbContext.UsuarioInfo.ToList();
            }
            catch
            {
                throw; 
            }
        }

        public Usuarios GetUsuarioDetails(int id)
        {
            try
            {
                Usuarios? usuario = _dbContext.UsuarioInfo.Find(id);
                if(usuario != null)
                {
                    return usuario;
                }
                else
                {
                    throw new ArgumentNullException();
                }
            }
            catch
            {
                throw;
            }
        }

        public bool ValidaUsuario(int id)
        {
            return _dbContext.UsuarioInfo.Any(e => e.id_usuario == id);
        }
    }
}
