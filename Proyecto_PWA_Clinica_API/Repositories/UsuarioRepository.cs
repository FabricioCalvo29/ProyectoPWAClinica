using Dapper;
using Microsoft.Data.SqlClient;
using Proyecto_PWA_Clinica_API.Models;
using System.Data;

namespace Proyecto_PWA_Clinica_API.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly IConfiguration _configuration;

        public UsuarioRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<RespuestaApi> RegistrarUsuario(UsuarioRegistroDto model)
        {
            try
            {
                using IDbConnection db = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parametros = new DynamicParameters();
                parametros.Add("@Nombre", model.Nombre);
                parametros.Add("@Apellido", model.Apellido);
                parametros.Add("@Correo", model.Correo);
                parametros.Add("@Contrasenna", model.Contrasena);
                parametros.Add("@FechaNacimiento", model.FechaNacimiento);
                parametros.Add("@Telefono", model.Telefono);
                parametros.Add("@TipoSangre", model.TipoSangre);
                parametros.Add("@HistorialMedico", model.HistorialMedico);

                var idUsuario = await db.ExecuteScalarAsync<int>(
                    "dbo.RegistrarPacienteDesdeLogin",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                return new RespuestaApi
                {
                    EsCorrecto = true,
                    Mensaje = $"Paciente registrado correctamente. IdUsuario generado: {idUsuario}"
                };
            }
            catch (Exception ex)
            {
                return new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = ex.Message
                };
            }
        }

        public async Task<UsuarioSesionDto?> IniciarSesion(LoginDto model)
        {
            using IDbConnection db = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@Correo", model.Correo);
            parametros.Add("@Contrasenna", model.Contrasena);

            using var multi = await db.QueryMultipleAsync(
                "dbo.IniciarSesion",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            var usuario = await multi.ReadFirstOrDefaultAsync<UsuarioSesionDto>();
            var roles = (await multi.ReadAsync<RolDto>()).ToList();

            if (usuario != null)
            {
                usuario.Roles = roles;
            }

            return usuario;
        }

        public async Task<bool> ValidarCorreoExiste(string correo)
        {
            using IDbConnection db = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@Correo", correo);

            var resultado = await db.QueryFirstOrDefaultAsync(
                "dbo.ValidarCorreo",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return resultado != null;
        }

        public async Task<bool> GuardarTokenRecuperacion(string correo, string token, DateTime vencimiento)
        {
            using IDbConnection db = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@Correo", correo);
            parametros.Add("@TokenRecuperacion", token);
            parametros.Add("@FechaVencimientoToken", vencimiento);

            await db.ExecuteAsync(
                "dbo.GuardarTokenRecuperacion",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }

        public async Task<TokenRecuperacionDto?> ValidarTokenRecuperacion(string token)
        {
            using IDbConnection db = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@TokenRecuperacion", token);

            return await db.QueryFirstOrDefaultAsync<TokenRecuperacionDto>(
                "dbo.ValidarTokenRecuperacion",
                parametros,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<bool> ActualizarContrasena(int idCredencial, string contrasenaEncriptada)
        {
            using IDbConnection db = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@IdCredencial", idCredencial);
            parametros.Add("@Contrasenna", contrasenaEncriptada);

            await db.ExecuteAsync(
                "dbo.ActualizarContrasenna",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return true;
        }
    }
}