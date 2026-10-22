namespace api_tienda_web_odi.Data.Chats
{
    public class ArchivosMensaje
    {
        public int Id { get; set; }
        public string NombreArchivo { get; set; } = string.Empty;
        public int MensajeId { get; set; }
        public MensajeChat Mensaje { get; set; } = new();
    }
}
