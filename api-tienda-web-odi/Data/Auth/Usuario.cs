using api_tienda_web_odi.Data.Chats;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_tienda_web_odi.Data.Auth
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        public string Correo { get; set; }
        public string Biografia { get; set; } = string.Empty;
        public string FotoPerfilUrl { get; set; } = "/Uploads/Usuarios/default.webp";
        public DateTime FechaRegistro { get; set; }
        public bool Activo { get; set; }
        public bool EmailConfirmado { get; set; }
        public required string PasswordHash { get; set; }
        public Rol Rol { get; set; }

        [InverseProperty("Vendedor")]
        public List<Producto.Producto> Productos { get; set; } = [];

        [InverseProperty("Comprador")]
        public virtual List<Chat> ChatsComprador { get; set; } = [];
    }
}
