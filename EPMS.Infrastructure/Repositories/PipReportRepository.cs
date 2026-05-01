using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
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
            ArgumentNullException.ThrowIfNull(config);
            _connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
        }

        private SqlConnection CreateConnection() => new(_connectionString);

        public async Task<IEnumerable<PipReportResult>> GetPipSummaryReportAsync(
            int? managerId = null,
            string? status = null,
            CancellationToken ct = default)
        {
            ct.ThrowIfCancellationRequested();

            await using var conn = CreateConnection();
            await conn.OpenAsync(ct);

            var param = new DynamicParameters();
            param.Add("@ManagerID", managerId, DbType.Int32);
            param.Add("@Status", status, DbType.String);

            return await conn.QueryAsync<PipReportResult>(
                new CommandDefinition(
                    "sp_GetPipSummaryReport",
                    param,
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct));
        }

        public async Task<PipReportResult?> GetPipDetailReportAsync(
            int pipId,
            CancellationToken ct = default)
        {
            if (pipId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(pipId));
            }

            ct.ThrowIfCancellationRequested();
            await using var conn = CreateConnection();
            await conn.OpenAsync(ct);

            return await conn.QueryFirstOrDefaultAsync<PipReportResult>(
                new CommandDefinition(
                    "sp_GetPipDetailReport",
                    new { PIPID = pipId },
                    commandType: CommandType.StoredProcedure,
                    cancellationToken: ct));
        }
    }
}