using Dapper;
using Microsoft.Data.SqlClient;
using Proyecto_PWA_Clinica_API.Models;
using System.Data;

namespace Proyecto_PWA_Clinica_API.Repositories
{
    public class CitaRepository : ICitaRepository
    {
        private readonly IConfiguration _configuration;

        public CitaRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<MedicoDisponibleDto>> ConsultarMedicosActivosParaCita()
        {
            using IDbConnection db = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            return await db.QueryAsync<MedicoDisponibleDto>(
                "dbo.ConsultarMedicosActivosParaCita",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<RespuestaApi> RegistrarCitaPaciente(RegistrarCitaDto model)
        {
            try
            {
                using IDbConnection db = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var parametros = new DynamicParameters();
                parametros.Add("@IdUsuario", model.IdUsuario);
                parametros.Add("@IdMedico", model.IdMedico);
                parametros.Add("@FechaHora", model.FechaHora);
                parametros.Add("@Motivo", model.Motivo);

                var idCita = await db.ExecuteScalarAsync<int>(
                    "dbo.RegistrarCitaPaciente",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                return new RespuestaApi
                {
                    EsCorrecto = true,
                    Mensaje = $"Cita registrada correctamente. IdCita generado: {idCita}"
                };
            }
            catch (SqlException ex) when (ex.Number == 2812)
            {
                return new RespuestaApi
                {
                    EsCorrecto = false,
                    Mensaje = "No existe el procedimiento dbo.RegistrarCitaPaciente. Debe ejecutar ClinicaDB.sql."
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

        public async Task<IEnumerable<CitaDto>> ConsultarCitasPaciente(int idUsuario)
        {
            using IDbConnection db = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            try
            {
                var parametros = new DynamicParameters();
                parametros.Add("@IdUsuario", idUsuario);

                return await db.QueryAsync<CitaDto>(
                    "dbo.ConsultarCitasPaciente",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (SqlException ex) when (ex.Number == 2812)
            {
                throw new InvalidOperationException(
                    "No existe el procedimiento dbo.ConsultarCitasPaciente. Debe ejecutar ClinicaDB.sql.",
                    ex
                );
            }
        }

        public async Task<CitaDto?> ConsultarDetalleCitaPaciente(int idUsuario, int idCita)
        {
            using IDbConnection db = new SqlConnection(
                _configuration.GetConnectionString("DefaultConnection"));

            try
            {
                var parametros = new DynamicParameters();
                parametros.Add("@IdUsuario", idUsuario);
                parametros.Add("@IdCita", idCita);

                return await db.QueryFirstOrDefaultAsync<CitaDto>(
                    "dbo.ConsultarDetalleCitaPaciente",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );
            }
            catch (SqlException ex) when (ex.Number == 2812)
            {
                throw new InvalidOperationException(
                    "No existe el procedimiento dbo.ConsultarDetalleCitaPaciente. Debe ejecutar ClinicaDB.sql.",
                    ex
                );
            }
        }
    }
}
