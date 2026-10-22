using api_tienda_web_odi.Data.Auth;
using api_tienda_web_odi.Data.Chats;
using api_tienda_web_odi.Data.Notificacion;
using api_tienda_web_odi.Data.Producto;
using Microsoft.EntityFrameworkCore;

namespace api_tienda_web_odi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        { 
        }

        #region Auth
        public DbSet<Usuario> Usuario { get; set; }
        #endregion

        #region Chats
        public DbSet<Chat> Chat { get; set; }
        public DbSet<MensajeChat> MensajeChat { get; set; }
        public DbSet<ArchivosMensaje> ArchivosMensaje { get; set; }
        #endregion

        #region Producto
        public DbSet<FotosProducto> FotosProducto { get; set; }
        public DbSet<Producto.Producto> Producto { get; set; }
        #endregion

        #region Notificaciones
        public DbSet<Notificaciones> Notificaciones { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Usuario -> Productos
            modelBuilder.Entity<Producto.Producto>()
                .HasOne(p => p.Vendedor)
                .WithMany(u => u.Productos)
                .HasForeignKey(p => p.VendedorId)
                .OnDelete(DeleteBehavior.Cascade);

            // Producto -> Chats
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Producto)
                .WithMany(p => p.Chats)
                .HasForeignKey(c => c.ProductoId)
                .OnDelete(DeleteBehavior.NoAction);

            // Usuario (El que pregunta por el producto) -> Chats
            modelBuilder.Entity<Chat>()
                .HasOne(c => c.Comprador)
                .WithMany(u => u.ChatsComprador)
                .HasForeignKey(c => c.CompradorId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
