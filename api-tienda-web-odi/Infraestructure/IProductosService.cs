using api_tienda_web_odi.Models.Productos;
using api_tienda_web_odi.Data.Producto;

namespace api_tienda_web_odi.Infraestructure
{
    public interface IProductosService
    {
        Task<bool> CrearProducto(CrearProductoDTO producto, Guid VendedorId);
        Task<bool> EliminarProducto(Guid productoId, Guid userId);
        Task<List<ProductoDTO>> ObtenerProductosDeUsuario(Guid UsuarioId);
        List<TipoTransaccionDTO> ObtenerTiposTransaccion();
        Task<PaginatedProductosDTO> BuscarProductos(
            double latitud,
            double longitud,
            double radio,
            int pagina = 1,
            int cantidadPorPagina = 10,
            int? categoriaId = null,
            TipoTransaccion? tipoTransaccion = null);
    }
}
