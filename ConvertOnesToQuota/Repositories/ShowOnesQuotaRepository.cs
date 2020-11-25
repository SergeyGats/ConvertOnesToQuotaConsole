using ConvertOnesToQuota.Common;
using ConvertOnesToQuota.Models;

namespace ConvertOnesToQuota.Repositories
{
    public class ShowOnesQuotaRepository : BaseRepository<ShowOnesQuota>
    {
        public ShowOnesQuotaRepository(ApplicationDatabaseContext context)
            : base(context)
        {

        }
    }
}
