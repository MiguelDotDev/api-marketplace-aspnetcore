namespace api_tienda_web_odi.Models.Productos
{
    public class InsertarFotoProductoDTO
    {
        public int Orden { get; set; }
        public IFormFile Foto { get; set; }
    }
}
