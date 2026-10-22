using api_tienda_web_odi.Infraestructure;
using api_tienda_web_odi.Models.Auth;
using api_tienda_web_odi.Wrapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace api_tienda_web_odi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            string? token = await _authService.Login(login);

            if (token == null)
            {
                return Unauthorized(new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Correo o contraseña incorrectos.",
                    Code = HttpStatusCode.Unauthorized
                });
            }

            return Ok(new ResponseWrapper<string>
            {
                Data = token,
                Message = "Login exitoso",
                Code = HttpStatusCode.OK
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO registerDto)
        {
            bool registrado = await _authService.RegisterAsync(registerDto);

            if (!registrado)
            {
                return BadRequest(new ResponseWrapper<bool>
                {
                    Data = false,
                    Message = "Ya existe un usuario con ese correo.",
                    Code = HttpStatusCode.BadRequest
                });
            }

            return Ok(new ResponseWrapper<bool>
            {
                Data = true,
                Message = "Usuario registrado correctamente.",
                Code = HttpStatusCode.OK
            });
        }
    }
}
