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

            try
            {
                return await db.QueryAsync<MedicoDisponibleDto>(
                    "dbo.ConsultarMedicosActivosParaCita",
                    commandType: CommandType.StoredProcedure
                );
            }
            catch
            {
                // Fallback para entornos donde el procedimiento aun no fue aplicado.
                const string sql = @"
                    SELECT
                        M.IdMedico,
                        CONCAT(U.Nombre, ' ', U.Apellido) AS NombreMedico,
                        ISNULL(E.Nombre, 'Sin especialidad') AS Especialidad
                    FROM dbo.tMedico M
                    INNER JOIN dbo.tUsuario U ON M.IdUsuario = U.IdUsuario
                    LEFT JOIN dbo.tEspecialidad E ON M.IdEspecialidad = E.IdEspecialidad
                    WHERE M.Estado = 1
                      AND U.Estado = 1
                    ORDER BY U.Nombre, U.Apellido;";

                return await db.QueryAsync<MedicoDisponibleDto>(sql);
            }
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
            catch (SqlException ex) when (ex.Message.Contains("Could not find stored procedure 'dbo.RegistrarCitaPaciente'"))
            {
                using IDbConnection db = new SqlConnection(
                    _configuration.GetConnectionString("DefaultConnection"));

                var idPaciente = await db.QueryFirstOrDefaultAsync<int?>(
                    @"SELECT TOP 1 P.IdPaciente
                      FROM dbo.tPaciente P
                      INNER JOIN dbo.tUsuario U ON P.IdUsuario = U.IdUsuario
                      WHERE P.IdUsuario = @IdUsuario
                        AND P.Estado = 1
                        AND U.Estado = 1;",
                    new { model.IdUsuario }
                );

                if (idPaciente == null)
                {
                    return new RespuestaApi
                    {
                        EsCorrecto = false,
                        Mensaje = "No existe un paciente activo para el usuario indicado."
                    };
                }

                var medicoActivo = await db.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1)
                      FROM dbo.tMedico M
                      INNER JOIN dbo.tUsuario U ON M.IdUsuario = U.IdUsuario
                      WHERE M.IdMedico = @IdMedico
                        AND M.Estado = 1
                        AND U.Estado = 1;",
                    new { model.IdMedico }
                );

                if (medicoActivo <= 0)
                {
                    return new RespuestaApi
                    {
                        EsCorrecto = false,
                        Mensaje = "El medico seleccionado no se encuentra disponible."
                    };
                }

                if (model.FechaHora <= DateTime.Now)
                {
                    return new RespuestaApi
                    {
                        EsCorrecto = false,
                        Mensaje = "La fecha y hora de la cita debe ser futura."
                    };
                }

                var medicoOcupado = await db.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1)
                      FROM dbo.tCita
                      WHERE IdMedico = @IdMedico
                        AND FechaHora = @FechaHora
                        AND EstadoCita = 'Pendiente';",
                    new { model.IdMedico, model.FechaHora }
                );

                if (medicoOcupado > 0)
                {
                    return new RespuestaApi
                    {
                        EsCorrecto = false,
                        Mensaje = "El medico ya tiene una cita pendiente en la fecha y hora indicada."
                    };
                }

                var pacienteOcupado = await db.ExecuteScalarAsync<int>(
                    @"SELECT COUNT(1)
                      FROM dbo.tCita
                      WHERE IdPaciente = @IdPaciente
                        AND FechaHora = @FechaHora
                        AND EstadoCita = 'Pendiente';",
                    new { IdPaciente = idPaciente.Value, model.FechaHora }
                );

                if (pacienteOcupado > 0)
                {
                    return new RespuestaApi
                    {
                        EsCorrecto = false,
                        Mensaje = "El paciente ya tiene una cita pendiente en la fecha y hora indicada."
                    };
                }

                var idCita = await db.ExecuteScalarAsync<int>(
                    @"INSERT INTO dbo.tCita
                      (
                          IdPaciente,
                          IdMedico,
                          FechaHora,
                          EstadoCita,
                          Motivo,
                          NotasMedico
                      )
                      VALUES
                      (
                          @IdPaciente,
                          @IdMedico,
                          @FechaHora,
                          'Pendiente',
                          @Motivo,
                          NULL
                      );
                      SELECT CAST(SCOPE_IDENTITY() AS INT);",
                    new
                    {
                        IdPaciente = idPaciente.Value,
                        model.IdMedico,
                        model.FechaHora,
                        model.Motivo
                    }
                );

                return new RespuestaApi
                {
                    EsCorrecto = true,
                    Mensaje = $"Cita registrada correctamente. IdCita generado: {idCita}"
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
            catch (SqlException ex) when (ex.Message.Contains("Could not find stored procedure 'dbo.ConsultarCitasPaciente'"))
            {
                const string sql = @"
                    SELECT
                        C.IdCita,
                        C.IdPaciente,
                        C.IdMedico,
                        C.FechaHora,
                        C.EstadoCita,
                        C.Motivo,
                        C.NotasMedico,
                        CONCAT(UM.Nombre, ' ', UM.Apellido) AS NombreMedico,
                        ISNULL(E.Nombre, 'Sin especialidad') AS Especialidad
                    FROM dbo.tCita C
                    INNER JOIN dbo.tPaciente P ON C.IdPaciente = P.IdPaciente
                    INNER JOIN dbo.tMedico M ON C.IdMedico = M.IdMedico
                    INNER JOIN dbo.tUsuario UM ON M.IdUsuario = UM.IdUsuario
                    LEFT JOIN dbo.tEspecialidad E ON M.IdEspecialidad = E.IdEspecialidad
                    WHERE P.IdUsuario = @IdUsuario
                      AND P.Estado = 1
                    ORDER BY C.FechaHora DESC;";

                return await db.QueryAsync<CitaDto>(sql, new { IdUsuario = idUsuario });
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
            catch (SqlException ex) when (ex.Message.Contains("Could not find stored procedure 'dbo.ConsultarDetalleCitaPaciente'"))
            {
                const string sql = @"
                    SELECT TOP 1
                        C.IdCita,
                        C.IdPaciente,
                        C.IdMedico,
                        C.FechaHora,
                        C.EstadoCita,
                        C.Motivo,
                        C.NotasMedico,
                        CONCAT(UM.Nombre, ' ', UM.Apellido) AS NombreMedico,
                        ISNULL(E.Nombre, 'Sin especialidad') AS Especialidad
                    FROM dbo.tCita C
                    INNER JOIN dbo.tPaciente P ON C.IdPaciente = P.IdPaciente
                    INNER JOIN dbo.tMedico M ON C.IdMedico = M.IdMedico
                    INNER JOIN dbo.tUsuario UM ON M.IdUsuario = UM.IdUsuario
                    LEFT JOIN dbo.tEspecialidad E ON M.IdEspecialidad = E.IdEspecialidad
                    WHERE C.IdCita = @IdCita
                      AND P.IdUsuario = @IdUsuario
                      AND P.Estado = 1;";

                return await db.QueryFirstOrDefaultAsync<CitaDto>(
                    sql,
                    new { IdUsuario = idUsuario, IdCita = idCita }
                );
            }
        }
    }
}
