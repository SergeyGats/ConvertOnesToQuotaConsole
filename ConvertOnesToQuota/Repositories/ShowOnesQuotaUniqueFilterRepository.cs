using ConvertOnesToQuota.Common;
using ConvertOnesToQuota.Models;

namespace ConvertOnesToQuota.Repositories
{
    public class ShowOnesQuotaUniqueFilterRepository : BaseRepository<ShowOnesQuotaUniqueFilter>
    {
        public ShowOnesQuotaUniqueFilterRepository(ApplicationDatabaseContext context)
            : base(context)
        {

        }
    }
}
