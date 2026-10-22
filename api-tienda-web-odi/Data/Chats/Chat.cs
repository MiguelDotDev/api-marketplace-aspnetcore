using api_tienda_web_odi.Data.Auth;
using api_tienda_web_odi.Data.Producto;
using Microsoft.EntityFrameworkCore;

namespace api_tienda_web_odi.Data.Chats
{
    public class Chat
    {
        public Guid Id { get; set; }
        public DateTime FechaCreacion { get; set; } = DateTime.Now;
        public Guid? ProductoId { get; set; }
        public Producto.Producto? Producto { get; set; }
        public Guid? CompradorId { get; set; }
        public Usuario? Comprador { get; set; }
        public string NombreProductoSnapshot { get; set; } = string.Empty;
        public string ImagenProductoSnapshot { get; set; } = string.Empty;
        public TipoTransaccion TipoTransaccionProductoSnapshot { get; set; }
        public bool VisibleParaComprador { get; set; } = true;
        public bool VisibleParaVendedor { get; set; } = true;

        [Precision(18, 2)]
        public decimal PrecioProductoSnapshot { get; set; }
        public List<MensajeChat> MensajeChat { get; set; } = new();
    }
}
