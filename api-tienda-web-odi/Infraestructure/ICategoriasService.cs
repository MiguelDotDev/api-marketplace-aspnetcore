using api_tienda_web_odi.Models.Productos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api_tienda_web_odi.Infraestructure
{
    public interface ICategoriasService
    {
        Task<bool> CrearCategoria(CategoriaDTO categoria);
        Task<List<CategoriaDTO>> ObtenerTodas();
        Task<bool> EliminarCategoria(int id);
        Task<bool> ActualizarCategoria(int id, string nombre);
    }
}
