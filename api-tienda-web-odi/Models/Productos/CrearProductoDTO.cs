using api_tienda_web_odi.Data.Producto;
using Microsoft.EntityFrameworkCore;

namespace api_tienda_web_odi.Models.Productos
{
    public class CrearProductoDTO
    {
        public string Titulo { get; set; }
        [Precision(18, 2)]
        public decimal Precio { get; set; }
        public string Descripcion { get; set; }
        public int CategoriaId { get; set; }
        public TipoTransaccion TipoTransaccion { get; set; }
        public double Latitud { get; set; }
        public double Longitud { get; set; }
        public List<InsertarFotoProductoDTO> Fotos { get; set; } = [];
    }
}
