using api_tienda_web_odi.Data;
using api_tienda_web_odi.Data.Auth;
using api_tienda_web_odi.Infraestructure;
using api_tienda_web_odi.Models.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace api_tienda_web_odi.Service
{
    public class AuthService: IAuthService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthService(
            IConfiguration configuration,
            AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string GenerateToken(Usuario usuario) {
            var JWTSection = _configuration.GetSection("JWT");
            var Issuer = JWTSection["Issuer"];
            var Audience = JWTSection["Audience"];
            var Key = JWTSection["Key"];
            var TiempoExpiracion = JWTSection["ExpirationMinutes"];

            if (string.IsNullOrEmpty(Issuer) || string.IsNullOrEmpty(Audience) || 
                string.IsNullOrEmpty(Key) || string.IsNullOrEmpty(TiempoExpiracion))
            {
                throw new Exception("JWT configuration is missing or invalid.");
            }

            var SecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
            var Credentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);

            var Claims = new[]
            {
                new Claim(ClaimTypes.Email, usuario.Correo),
                new Claim(ClaimTypes.Name, usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.Rol.ToString()),
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString())
            };

            var Token = new JwtSecurityToken(
                issuer: Issuer,
                audience: Audience,
                claims: Claims,
                signingCredentials: Credentials,
                expires: DateTime.UtcNow.AddMinutes(double.Parse(TiempoExpiracion))
            );

            var TokenString = new JwtSecurityTokenHandler().WriteToken(Token);

            return TokenString;
        }

        public async Task<string?> Login(LoginDTO login)
        {
            Usuario? usuario = await _context.Usuario.FirstOrDefaultAsync(u => u.Correo == login.Correo);

            if (usuario == null)
                return null;

            if (!BCrypt.Net.BCrypt.Verify(login.Password, usuario.PasswordHash))
                return null;

            return GenerateToken(usuario);
        }

        public async Task<bool> RegisterAsync(RegisterDTO registerDto)
        {
            bool existe = await _context.Usuario
                .AnyAsync(u => u.Correo == registerDto.Correo);

            if (existe)
                return false;

            Usuario usuario = new()
            {
                Id = Guid.NewGuid(),
                Nombre = registerDto.Nombre,
                ApellidoPaterno = registerDto.ApellidoPaterno,
                ApellidoMaterno = registerDto.ApellidoMaterno,
                Correo = registerDto.Correo,

                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),

                FechaRegistro = DateTime.UtcNow,
                Activo = true,
                EmailConfirmado = false,

                Rol = Rol.Usuario
            };

            await _context.Usuario.AddAsync(usuario);
            await _context.SaveChangesAsync();

            return true;
        }

    }
}
