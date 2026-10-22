using api_tienda_web_odi.Data.Auth;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_tienda_web_odi.Data.Chats
{
    public class MensajeChat
    {
        public int Id { get; set; }
        public Guid ChatId { get; set; }
        public Chat Chat { get; set; } = new();
        public string Contenido { get; set; } = string.Empty;
        public DateTime FechaEnvio { get; set; } = DateTime.UtcNow;
        public EmisorMensaje Emisor { get; set; } = 0;
        public EstadoMensaje Estado { get; set; } = 0;
        [InverseProperty("Mensaje")]
        public List<ArchivosMensaje> Archivos { get; set; } = new();
    }
}
