using EPMS.Domain.SpResults;

namespace EPMS.Domain.Interfaces
{
    public interface IPipReportRepository
    {
        Task<IEnumerable<PipReportResult>> GetPipSummaryReportAsync(
            int? managerId = null,
            string? status = null,
            CancellationToken ct = default);

        Task<PipReportResult?> GetPipDetailReportAsync(int pipId, CancellationToken ct = default);
    }
}
