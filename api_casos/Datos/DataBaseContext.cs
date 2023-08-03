using api_casos.Modelos;
using Microsoft.EntityFrameworkCore;

namespace api_casos.Datos
{
    public partial class DataBaseContext: DbContext
    {
        public DataBaseContext()
        {
            
        }
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
            
        }

        public virtual DbSet<Usuarios>? UsuarioInfo { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Usuarios>(entity => 
            {
                entity.HasNoKey();
                entity.ToTable("usuarios");
                entity.Property(e => e.id_usuario).HasColumnName("id_usuario");
                entity.Property(e => e.nombre).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.apellido1).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.apellido2).HasMaxLength(100).IsUnicode(false);
                entity.Property(e => e.identificacion).HasMaxLength(30).IsUnicode(false);
                entity.Property(e => e.usuario).HasMaxLength(30).IsUnicode(false);
                entity.Property(e => e.correo).HasMaxLength(150).IsUnicode(false);
                entity.Property(e => e.dpto_original).HasMaxLength(30).IsUnicode(false);
                entity.Property(e => e.dpto_adicional).HasMaxLength(30).IsUnicode(false);
                entity.Property(e => e.clave).HasMaxLength(60).IsUnicode(false);
                entity.Property(e => e.fecha_ultimo_cambio_clave).IsUnicode(false);
                entity.Property(e => e.rol).HasMaxLength(60).IsUnicode(false);
                entity.Property(e => e.fecha_registro).IsUnicode(false);
                entity.Property(e => e.usuario_registra).HasMaxLength(60).IsUnicode(false); ;
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

    }
}
