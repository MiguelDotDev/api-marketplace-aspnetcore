using System.ComponentModel.DataAnnotations.Schema;

namespace api_tienda_web_odi.Data.Producto
{
    public class Categoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public List<Producto> Productos { get; set; } = new List<Producto>();
    }
}
