using api_casos.Modelos;

namespace api_casos.Datos.Interfaces
{
    public interface IUsuario
    {
        public List<Usuarios> GetUsuarioDetails();
        public Usuarios GetUsuarioDetails(int id);
        public void AgregaUsuario(Usuarios usuario);
        public void ActualizaUsuario(Usuarios usuario);
        public Usuarios BorrarUsuario(int id);
        public bool ValidaUsuario(int id);
    }
}
