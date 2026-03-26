using Proyecto_PWA_Clinica_API.Models;

namespace Proyecto_PWA_Clinica_API.Repositories
{
    public interface IUsuarioRepository
    {
        Task<RespuestaApi> RegistrarUsuario(UsuarioRegistroDto model);
        Task<UsuarioSesionDto?> IniciarSesion(LoginDto model);

        Task<bool> ValidarCorreoExiste(string correo);
        Task<bool> GuardarTokenRecuperacion(string correo, string token, DateTime vencimiento);
        Task<TokenRecuperacionDto?> ValidarTokenRecuperacion(string token);
        Task<bool> ActualizarContrasena(int idCredencial, string contrasenaEncriptada);
    }
}