using api_tienda_web_odi.Models.Chats;

namespace api_tienda_web_odi.Infraestructure
{
    public interface IChatsService
    {
        Task<bool> BorrarChat(Guid UsuarioId, Guid ChatId);
        Task<bool> CrearChat(Guid InteresadoId, Guid ProductoId);
        Task<bool> EnviarMensaje(CrearMensajeDTO Mensaje, Guid EmisorId);
        Task<List<ChatDTO>> ObtenerChats(Guid UsuarioId, int iteracion = 0);
    }
}
