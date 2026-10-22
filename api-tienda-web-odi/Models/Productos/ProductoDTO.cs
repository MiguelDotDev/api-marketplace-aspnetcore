using api_tienda_web_odi.Data.Auth;
using api_tienda_web_odi.Data.Chats;
using api_tienda_web_odi.Data.Producto;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_tienda_web_odi.Models.Productos
{
    public class ProductoDTO
    {
        public Guid Id { get; set; }
        public string Titulo { get; set; }

        [Precision(18, 2)]
        public decimal Precio { get; set; }
        public string Descripcion { get; set; }
        public bool Disponible { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public TipoTransaccion TipoTransaccion { get; set; }
        public Guid VendedorId { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public List<IFormFile> Fotos { get; set; } = [];
    }
}
