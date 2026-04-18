using Dapper;
using Microsoft.Data.SqlClient;
using Proyecto_PWA_Clinica_API.Models;
using System.Data;

namespace Proyecto_PWA_Clinica_API.Repositories
{
    public class MedicoRepository : IMedicoRepository
    {
        private readonly IConfiguration _configuration;

        public MedicoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<EspecialidadDto>> ConsultarEspecialidadesActivas()
        {
            using IDbConnection db = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return await db.QueryAsync<EspecialidadDto>(
                "dbo.ConsultarEspecialidadesActivas",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<IEnumerable<MedicoAdminDto>> ConsultarTodosLosMedicos()
        {
            using IDbConnection db = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return await db.QueryAsync<MedicoAdminDto>(
                "dbo.ConsultarTodosLosMedicos",
                commandType: CommandType.StoredProcedure);
        }

        public async Task<MedicoAdminDto?> ConsultarDetalleMedico(int idMedico)
        {
            using IDbConnection db = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@IdMedico", idMedico);

            return await db.QueryFirstOrDefaultAsync<MedicoAdminDto>(
                "dbo.ConsultarDetalleMedico",
                parametros,
                commandType: CommandType.StoredProcedure);
        }

        public async Task<RespuestaApi> RegistrarMedicoAdmin(RegistrarMedicoAdminDto model)
        {
            try
            {
                using IDbConnection db = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parametros = new DynamicParameters();
                parametros.Add("@Nombre", model.Nombre);
                parametros.Add("@Apellido", model.Apellido);
                parametros.Add("@Correo", model.Correo);
                parametros.Add("@Contrasenna", model.Contrasenna);
                parametros.Add("@IdEspecialidad", model.IdEspecialidad);

                await db.ExecuteAsync(
                    "dbo.RegistrarMedicoAdmin",
                    parametros,
                    commandType: CommandType.StoredProcedure);

                return new RespuestaApi
                {
                    EsCorrecto = true,
                    Mensaje = "Médico registrado correctamente."
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

        public async Task<RespuestaApi> ActualizarMedicoAdmin(RegistrarMedicoAdminDto model)
        {
            try
            {
                using IDbConnection db = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parametros = new DynamicParameters();
                parametros.Add("@IdMedico", model.IdMedico);
                parametros.Add("@Nombre", model.Nombre);
                parametros.Add("@Apellido", model.Apellido);
                parametros.Add("@Correo", model.Correo);
                parametros.Add("@IdEspecialidad", model.IdEspecialidad);
                parametros.Add("@CodigoProfesional", model.CodigoProfesional);

                await db.ExecuteAsync(
                    "dbo.ActualizarMedicoAdmin",
                    parametros,
                    commandType: CommandType.StoredProcedure);

                return new RespuestaApi
                {
                    EsCorrecto = true,
                    Mensaje = "Médico actualizado correctamente."
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

        public async Task<RespuestaApi> CambiarEstadoMedicoAdmin(int idMedico, bool estado)
        {
            try
            {
                using IDbConnection db = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parametros = new DynamicParameters();
                parametros.Add("@IdMedico", idMedico);
                parametros.Add("@Estado", estado);

                await db.ExecuteAsync(
                    "dbo.CambiarEstadoMedicoAdmin",
                    parametros,
                    commandType: CommandType.StoredProcedure);

                return new RespuestaApi
                {
                    EsCorrecto = true,
                    Mensaje = estado
                        ? "Médico activado correctamente."
                        : "Médico inactivado correctamente."
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
    }
}