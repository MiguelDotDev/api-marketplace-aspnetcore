using api_tienda_web_odi.Data;
using api_tienda_web_odi.Data.Chats;
using api_tienda_web_odi.Data.Producto;
using api_tienda_web_odi.Infraestructure;
using api_tienda_web_odi.Models.Chats;
using Microsoft.EntityFrameworkCore;

namespace api_tienda_web_odi.Service
{
    public class ChatsService : IChatsService
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public ChatsService(
            AppDbContext context, 
            IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }
        public async Task<bool> BorrarChat(Guid UsuarioId, Guid ChatId)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var chat = await _context.Chat.FirstOrDefaultAsync(x => x.Id == ChatId);
                if (chat == null)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                if (chat.CompradorId == UsuarioId)
                {
                    if (chat.VisibleParaVendedor)
                    {
                        chat.VisibleParaComprador = false;

                        var result = await _context.SaveChangesAsync();
                        if (result > 0)
                        {
                            await transaction.CommitAsync();
                            return true;
                        }
                        return false;
                    } 
                    else
                    {
                        _context.Chat.Remove(chat);
                        var result = await _context.SaveChangesAsync();
                        if (result > 0)
                        {
                            await transaction.CommitAsync();
                            var carpetaChat = Path.Combine(
                                _environment.ContentRootPath,
                                "PrivateUserFiles",
                                "ChatsUsuarios",
                                chat.Id.ToString()
                            );
                            if (Directory.Exists(carpetaChat))
                                Directory.Delete(carpetaChat, true);
                            return true;
                        }
                        return false;
                    }
                }
                else
                {
                    var producto = await _context.Producto.FirstOrDefaultAsync(x => x.Id == chat.ProductoId);
                    if (producto == null)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }

                    if (producto.VendedorId == UsuarioId)
                    {
                        if (chat.VisibleParaComprador)
                        {
                            chat.VisibleParaVendedor = false;
                            var result = await _context.SaveChangesAsync();
                            if (result > 0)
                            {
                                await transaction.CommitAsync();
                                return true;
                            }
                            return false;
                        }
                        else
                        {
                            _context.Chat.Remove(chat);
                            var result = await _context.SaveChangesAsync();
                            if (result > 0)
                            {
                                await transaction.CommitAsync();
                                var carpetaChat = Path.Combine(
                                    _environment.ContentRootPath,
                                    "PrivateUserFiles",
                                    "ChatsUsuarios",
                                    chat.Id.ToString()
                                );
                                if (Directory.Exists(carpetaChat))
                                    Directory.Delete(carpetaChat, true);
                                return true;
                            }
                            return false;
                        }
                    }
                    await transaction.RollbackAsync();
                    return false;
                }
            }
            catch
            {
                await transaction.RollbackAsync();
                return false;
            }
        }

        public async Task<List<ChatDTO>> ObtenerChats(Guid UsuarioId, int iteracion = 0)
        {
            int cantidadPorPagina = 10;
            int saltar = iteracion * cantidadPorPagina;

            var chats = await (
                from c in _context.Chat
                join p in _context.Producto on c.ProductoId equals p.Id
                join m in _context.MensajeChat on c.Id equals m.ChatId into mensajes
                join comprador in _context.Usuario on c.CompradorId equals comprador.Id //Usuario comprador (El que inicia el chat)
                join vendedor in _context.Usuario on p.VendedorId equals vendedor.Id //Usuario vendedor (El que responde al chat)
                where 
                    (c.CompradorId == UsuarioId && c.VisibleParaComprador) || 
                    (p.VendedorId == UsuarioId && c.VisibleParaVendedor)
                select new ChatDTO
                {
                    Id = c.Id,
                    ImagenProductoSnapshot = c.ImagenProductoSnapshot,
                    NombreProductoSnapshot = c.NombreProductoSnapshot,
                    TipoTransaccionProductoSnapshot = c.TipoTransaccionProductoSnapshot,
                    ProductoId = p.Id,
                    UrlFotoUsuario = UsuarioId == c.CompradorId ? vendedor.FotoPerfilUrl : comprador.FotoPerfilUrl,
                    UltimoMensaje = mensajes.OrderByDescending(x => x.FechaEnvio).Select(x => new MensajeDTO
                    {
                        Id = x.Id,
                        Contenido = x.Contenido,
                        FechaEnvio = x.FechaEnvio,
                        Emisor = x.Emisor,
                        Estado = x.Estado
                    }).FirstOrDefault(),
                    UltimoMovimiento = mensajes.Any() ? mensajes.Max(x => x.FechaEnvio) : c.FechaCreacion
                })
                .OrderByDescending(x => x.UltimoMovimiento)
                .Skip(saltar)
                .Take(cantidadPorPagina)
                .ToListAsync();

            return chats;
        }

        public async Task<bool> CrearChat(Guid InteresadoId, Guid ProductoId)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var producto = await _context.Producto.FirstOrDefaultAsync(x => x.Id == ProductoId);

                _context.Chat.Add(new Chat
                {
                    CompradorId = InteresadoId,
                    ProductoId = ProductoId,
                    ImagenProductoSnapshot = "",
                    NombreProductoSnapshot = producto.Titulo,
                    TipoTransaccionProductoSnapshot = producto.TipoTransaccion,
                    
                });

                var result = await _context.SaveChangesAsync();
                if (result <= 0) {
                    transaction.Rollback();
                    return false;
                }
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _ = ex;
                transaction.Rollback();
                return false;
            }
        }

        public async Task<bool> EnviarMensaje(CrearMensajeDTO Mensaje, Guid EmisorId)
        {
            var transaction = await _context.Database.BeginTransactionAsync();
            var carpetaArchivo = string.Empty;
            try
            {
                //Saber si es Vendedor o Comprador quien envia el mensaje
                EmisorMensaje ValorEmisorMensaje = EmisorMensaje.Sistema; //Empieza en el supuesto que es un mensaje de Sistema

                if (!Mensaje.EsSistema) //Si no es del sistema vamos a determinar de quien es
                {
                    var chatOrigen = await _context.Chat
                        .Include(c => c.Producto)
                        .FirstOrDefaultAsync(x => x.Id == Mensaje.ChatId);

                    if (chatOrigen == null || chatOrigen.Producto == null)
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                    if (chatOrigen.CompradorId == EmisorId)
                    {
                        ValorEmisorMensaje = EmisorMensaje.Comprador;

                    }
                    else if (chatOrigen.Producto.VendedorId == EmisorId)
                    {
                        ValorEmisorMensaje = EmisorMensaje.Vendedor;
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        return false;
                    }
                }

                //Crear el mensaje en BD
                var MensajeBD = await _context.MensajeChat.AddAsync(new MensajeChat
                {
                    ChatId = Mensaje.ChatId,
                    Estado = 0,
                    Contenido = Mensaje.Mensaje,
                    Emisor = ValorEmisorMensaje
                });

                var result = await _context.SaveChangesAsync();
                if (result <= 0)
                {
                    await transaction.RollbackAsync();
                    return false;
                }

                //Agregar los archivos al mensaje si los hay
                if (Mensaje.Archivos != null && Mensaje.Archivos.Any())
                {
                    var contador = 0;
                    foreach (var archivo in Mensaje.Archivos)
                    {
                        contador++;
                        string nombreArchivo =
                            $"{contador}{Path.GetExtension(archivo.FileName)}";

                        carpetaArchivo = Path.Combine(
                            _environment.ContentRootPath,
                            "PrivateUserFiles",
                            "ChatsUsuarios",
                            Mensaje.ChatId.ToString(),
                            MensajeBD.Entity.Id.ToString()
                        );

                        Directory.CreateDirectory(carpetaArchivo);

                        string rutaFisica = Path.Combine(
                            carpetaArchivo,
                            nombreArchivo
                        );

                        // Guarda el archivo en disco
                        await using (FileStream stream = new(rutaFisica, FileMode.Create))
                        {
                            await archivo.CopyToAsync(stream);
                        }

                        _context.ArchivosMensaje.Add(new ArchivosMensaje
                        {
                            MensajeId = MensajeBD.Entity.Id,
                            NombreArchivo = nombreArchivo
                        });
                    }
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return true;
            }
            catch (Exception ex)
            {
                _ = ex;
                await transaction.RollbackAsync();
                if (!string.IsNullOrWhiteSpace(carpetaArchivo)) Directory.Delete(carpetaArchivo, true);
                return false;
            }
        }
    }
}
