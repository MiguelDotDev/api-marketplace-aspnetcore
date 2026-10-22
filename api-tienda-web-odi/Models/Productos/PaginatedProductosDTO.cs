namespace api_tienda_web_odi.Models.Productos
{
    public class PaginatedProductosDTO
    {
        public List<ProductoDTO> Productos { get; set; } = new();
        public int PaginaActual { get; set; }
        public int CantidadPorPagina { get; set; }
        public int TotalRegistros { get; set; }
        public int TotalPaginas { get; set; }
    }
}
