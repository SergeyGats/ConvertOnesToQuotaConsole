using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ConvertOnesToQuota.Common;
using ConvertOnesToQuota.Models;

namespace ConvertOnesToQuota.Repositories
{
    public class ShowRepository : BaseRepository<Show>
    {
        public ShowRepository(ApplicationDatabaseContext context)
            : base(context)
        {

        }

        public List<Show> GetShowsByBuId(int buId)
        {
            var shows = GetCollectionAsQueryable(s => s.IsShow && !s.IsDeleted && s.BuId == buId)
                .AsNoTracking()
                .ToList();

            return shows;
        }
    }
}
