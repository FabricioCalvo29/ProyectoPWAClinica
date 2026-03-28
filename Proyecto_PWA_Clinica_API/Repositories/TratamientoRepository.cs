using Dapper;
using Microsoft.Data.SqlClient;
using Proyecto_PWA_Clinica_API.Models;
using System.Data;

namespace Proyecto_PWA_Clinica_API.Repositories
{
    public class TratamientoRepository : ITratamientoRepository
    {
        private readonly IConfiguration _configuration;

        public TratamientoRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<RespuestaApi> RegistrarTratamiento(RegistrarTratamientoDto model)
        {
            try
            {
                using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

                var parametros = new DynamicParameters();
                parametros.Add("@IdCita", model.IdCita);
                parametros.Add("@Medicamento", model.Medicamento);
                parametros.Add("@Dosis", model.Dosis);
                parametros.Add("@Duracion", model.Duracion);
                parametros.Add("@Instrucciones", model.Instrucciones);

                var idTratamiento = await db.ExecuteScalarAsync<int>(
                    "dbo.RegistrarTratamiento",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                return new RespuestaApi
                {
                    EsCorrecto = true,
                    Mensaje = $"Tratamiento registrado correctamente. Id generado: {idTratamiento}"
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

        public async Task<IEnumerable<TratamientoDto>> ConsultarTratamientosPorCita(int idCita)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@IdCita", idCita);

            return await db.QueryAsync<TratamientoDto>(
                "dbo.ConsultarTratamientosPorCita",
                parametros,
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<IEnumerable<TratamientoDto>> ConsultarTratamientosPaciente(int idUsuario)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@IdUsuario", idUsuario);

            return await db.QueryAsync<TratamientoDto>(
                "dbo.ConsultarTratamientosPaciente",
                parametros,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
