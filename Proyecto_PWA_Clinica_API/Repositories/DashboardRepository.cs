using Dapper;
using Microsoft.Data.SqlClient;
using Proyecto_PWA_Clinica_API.Models;
using System.Data;

namespace Proyecto_PWA_Clinica_API.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly IConfiguration _configuration;

        public DashboardRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<EstadisticasAdminDto> ConsultarEstadisticasAdmin()
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var resultado = await db.QueryFirstOrDefaultAsync<EstadisticasAdminDto>(
                "dbo.ConsultarEstadisticasAdmin",
                commandType: CommandType.StoredProcedure
            );

            return resultado ?? new EstadisticasAdminDto();
        }

        public async Task<EstadisticasPacienteDto> ConsultarEstadisticasPaciente(int idUsuario)
        {
            using IDbConnection db = new SqlConnection(_configuration.GetConnectionString("DefaultConnection"));

            var parametros = new DynamicParameters();
            parametros.Add("@IdUsuario", idUsuario);

            var resultado = await db.QueryFirstOrDefaultAsync<EstadisticasPacienteDto>(
                "dbo.ConsultarEstadisticasPaciente",
                parametros,
                commandType: CommandType.StoredProcedure
            );

            return resultado ?? new EstadisticasPacienteDto();
        }
    }
}
