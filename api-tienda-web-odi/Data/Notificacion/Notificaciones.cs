using api_tienda_web_odi.Data.Auth;

namespace api_tienda_web_odi.Data.Notificacion
{
    public class Notificaciones
    {
        public int Id { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Contenido { get; set; } = string.Empty;
        public Guid UsuarioNotificadoId { get; set; }
        public Usuario UsuarioNotificado { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public string UrlImagenIcono { get; set; } = string.Empty;
    }
}
