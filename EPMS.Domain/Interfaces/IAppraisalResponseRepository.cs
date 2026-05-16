using EPMS.Domain.Entities;

namespace EPMS.Domain.Interfaces;

public interface IAppraisalResponseRepository : IBaseRepository<AppraisalResponse, long>
{
    Task<IEnumerable<AppraisalResponse>> GetByEvalIdAsync(int evalId);
}
