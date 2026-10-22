namespace api_tienda_web_odi.Data.Producto
{
    public class FotosProducto
    {
        public int Id { get; set; }
        public Guid ProductoId { get; set; }
        public Producto Producto { get; set; } = new Producto();
        public string FotoRuta { get; set; } = string.Empty;
        public int Orden { get; set; }
    }
}
