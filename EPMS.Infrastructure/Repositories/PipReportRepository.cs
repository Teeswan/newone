using Dapper;
using EPMS.Domain.Interfaces;
using EPMS.Domain.SpResults;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace EPMS.Infrastructure.Repositories
{
    public sealed class PipReportRepository : IPipReportRepository
    {
        private readonly string _connectionString;

        public PipReportRepository(IConfiguration config)
        {
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private SqlConnection CreateConnection() => new(_connectionString);

        public async Task<IEnumerable<PipReportResult>> GetPipSummaryReportAsync(
            int? managerId = null,
            string? status = null,
            CancellationToken ct = default)
        {
            await using var conn = CreateConnection();
            var param = new DynamicParameters();
            param.Add("@ManagerID", managerId);
            param.Add("@Status", status);

            return await conn.QueryAsync<PipReportResult>(
                "sp_GetPipSummaryReport",
                param,
                commandType: System.Data.CommandType.StoredProcedure);
        }

        public async Task<PipReportResult?> GetPipDetailReportAsync(
            int pipId,
            CancellationToken ct = default)
        {
            await using var conn = CreateConnection();

            return await conn.QueryFirstOrDefaultAsync<PipReportResult>(
                "sp_GetPipDetailReport",
                new { PIPID = pipId },
                commandType: System.Data.CommandType.StoredProcedure);
        }
    }
}