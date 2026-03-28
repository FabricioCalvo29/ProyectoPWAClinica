using Dapper;
using Microsoft.Data.SqlClient;
using Proyecto_PWA_Clinica_API.Models;
using System.Data;

namespace Proyecto_PWA_Clinica_API.Repositories
{
    public class PacienteRepository : IPacienteRepository
    {
        private readonly IConfiguration _configuration;

        public PacienteRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<IEnumerable<PacienteListaDto>> ConsultarTodosLosPacientes()
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            return await db.QueryAsync<PacienteListaDto>(
                "dbo.ConsultarTodosLosPacientes",
                commandType: CommandType.StoredProcedure
            );
        }

        public async Task<PacienteListaDto?> ConsultarDetallePaciente(int idPaciente)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@IdPaciente", idPaciente);

            return await db.QueryFirstOrDefaultAsync<PacienteListaDto>(
                "dbo.ConsultarDetallePaciente",
                parametros,
                commandType: CommandType.StoredProcedure
            );
        }
    }
}
