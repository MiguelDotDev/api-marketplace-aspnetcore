namespace api_tienda_web_odi.Models.Auth
{
    public class RegisterDTO
    {
        public required string Nombre { get; set; }
        public required string ApellidoPaterno { get; set; }
        public required string ApellidoMaterno { get; set; }
        public required string Correo { get; set; }
        public required string Password { get; set; }
    }
}
