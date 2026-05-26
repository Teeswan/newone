using EPMS.Domain.Entities;
using EPMS.Domain.Interfaces;
using EPMS.Infrastructure.Contexts;

namespace EPMS.Infrastructure.Repositories;

public class RatingScaleRepository : BaseRepository<RatingScale, int>, IRatingScaleRepository
{
    public RatingScaleRepository(AppDbContext context) : base(context)
    {
    }
}
