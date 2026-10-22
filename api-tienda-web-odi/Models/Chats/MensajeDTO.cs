using api_tienda_web_odi.Data.Chats;

namespace api_tienda_web_odi.Models.Chats
{
    public class MensajeDTO
    {
        public int Id { get; set; }
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
        public EmisorMensaje Emisor { get; set; } = 0;
        public EstadoMensaje Estado { get; set; } = 0;
        public bool TieneArchivos { get; set; } = false;
    }
}
