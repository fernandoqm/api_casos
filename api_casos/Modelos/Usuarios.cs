namespace api_casos.Modelos
{
    public class Usuarios
    {
        public int id_usuario { get; set; }
        public string? nombre { get; set; }
        public string? apellido1 { get; set; }
        public string? apellido2 { get; set; }
        public string? identificacion { get; set; }
        public string? usuario { get; set; }
        public string? correo { get; set; } 
        public string? dpto_original { get; set; } 
        public string? dpto_adicional { get; set;}
        public string? clave { get; set; }
        public DateTime? fecha_ultimo_cambio_clave { get; set;}
        public string? rol { get; set; }
        public DateTime? fecha_registro { get; set; }
        public string? usuario_registra { get; set;}


    }
}
