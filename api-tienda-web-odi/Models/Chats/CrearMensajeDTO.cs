namespace api_tienda_web_odi.Models.Chats
{
    public class CrearMensajeDTO
    {
        public Guid ChatId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public bool EsSistema { get; set; } = false;
        public List<IFormFile> Archivos { get; set; } = new List<IFormFile>();
    }
}
