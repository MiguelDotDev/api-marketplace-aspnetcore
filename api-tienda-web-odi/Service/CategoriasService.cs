using api_tienda_web_odi.Data;
using api_tienda_web_odi.Infraestructure;
using api_tienda_web_odi.Models.Productos;
using api_tienda_web_odi.Data.Producto;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api_tienda_web_odi.Service
{
    public class CategoriasService : ICategoriasService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public CategoriasService(
            AppDbContext context,
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public async Task<bool> CrearCategoria(CategoriaDTO categoria)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = new Categoria
                {
                    Nombre = categoria.Nombre
                };

                await _context.Set<Categoria>().AddAsync(entity);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<List<CategoriaDTO>> ObtenerTodas()
        {
            var categorias = await _context.Set<Categoria>()
                .AsNoTracking()
                .Select(c => new CategoriaDTO { Id = c.Id, Nombre = c.Nombre })
                .ToListAsync();

            return categorias;
        }

        public async Task<bool> EliminarCategoria(int id)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = await _context.Set<Categoria>().FindAsync(id);
                if (entity == null)
                {
                    return false;
                }

                _context.Set<Categoria>().Remove(entity);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<bool> ActualizarCategoria(int id, string nombre)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var entity = await _context.Set<Categoria>().FindAsync(id);
                if (entity == null)
                {
                    return false;
                }

                entity.Nombre = nombre;
                _context.Set<Categoria>().Update(entity);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
                return true;
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

    }
}
