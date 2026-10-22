using api_tienda_web_odi.Infraestructure;
using api_tienda_web_odi.Models.Productos;
using api_tienda_web_odi.Wrapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using api_tienda_web_odi.Data.Auth;


namespace api_tienda_web_odi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriasService _categoriasService;
        public CategoriasController(ICategoriasService categoriasService)
        {
            _categoriasService = categoriasService;
        }

        [HttpPost("Crear")]
        [Authorize(nameof(Rol.Administrador))]
        public async Task<IActionResult> Crear([FromBody] CategoriaDTO categoria)
        {
            var creado = await _categoriasService.CrearCategoria(categoria);
            if (!creado)
            {
                return BadRequest(new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Error al crear la categoría.",
                    Code = HttpStatusCode.BadRequest
                });
            }
            return Ok(new ResponseWrapper<bool>
            {
                Data = true,
                Message = "Categoría creada correctamente.",
                Code = HttpStatusCode.OK
            });
        }

        [HttpGet("ObtenerTodas")]
        [Authorize]
        public async Task<IActionResult> ObtenerTodas()
        {
            var categorias = await _categoriasService.ObtenerTodas();
            return Ok(new ResponseWrapper<List<CategoriaDTO>>
            {
                Data = categorias,
                Message = "Categorías obtenidas correctamente.",
                Code = HttpStatusCode.OK
            });
        }

        [HttpDelete("Eliminar/{id}")]
        [Authorize(nameof(Rol.Administrador))]
        public async Task<IActionResult> Eliminar([FromRoute] int id)
        {
            var borrado = await _categoriasService.EliminarCategoria(id);
            if (!borrado)
            {
                return BadRequest(new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Error al eliminar la categoría.",
                    Code = HttpStatusCode.BadRequest
                });
            }
            return Ok(new ResponseWrapper<bool>
            {
                Data = true,
                Message = "Categoría eliminada correctamente.",
                Code = HttpStatusCode.OK
            });
        }

        [HttpPut("Actualizar/{id}")]
        [Authorize(nameof(Rol.Administrador))]
        public async Task<IActionResult> Actualizar([FromRoute] int id, [FromBody] string nombre)
        {
            var actualizado = await _categoriasService.ActualizarCategoria(id, nombre);
            if (!actualizado)
            {
                return BadRequest(new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Error al actualizar la categoría.",
                    Code = HttpStatusCode.BadRequest
                });
            }
            return Ok(new ResponseWrapper<bool>
            {
                Data = true,
                Message = "Categoría actualizada correctamente.",
                Code = HttpStatusCode.OK
            });
        }
    }
}
