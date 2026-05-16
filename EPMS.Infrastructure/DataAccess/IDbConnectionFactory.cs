using System.Data;

namespace EPMS.Infrastructure.DataAccess;

public interface IDbConnectionFactory
{
    IDbConnection CreateConnection();
}
