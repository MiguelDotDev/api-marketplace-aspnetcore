using api_tienda_web_odi.Data.Auth;
using api_tienda_web_odi.Models.Auth;

namespace api_tienda_web_odi.Infraestructure
{
    public interface IAuthService
    {
        string GenerateToken(Usuario usuario);
        Task<string?> Login(LoginDTO login);
        Task<bool> RegisterAsync(RegisterDTO registerDto);
    }
}
