using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using ConvertOnesToQuota.Common;
using ConvertOnesToQuota.Constants;
using ConvertOnesToQuota.Filters;
using ConvertOnesToQuota.Models;

namespace ConvertOnesToQuota.Repositories
{
    public class OnesRepository : BaseRepository<Ones>
    {


        public OnesRepository(ApplicationDatabaseContext context)
            : base(context)
        {

        }

        public IEnumerable<Ones> GetOnesForMasterScenario(OnesForMasterScenarioFilter onesFilter)
        {
            var ones = GetCollectionAsQueryable(o => o.ScenarioId == onesFilter.ShowOnesScenarioId &&
                    o.ShowId == onesFilter.ShowId &&
                    o.DisciplineId > 0 &&
                    o.ExtractDataType.Contains(OnesConstants.ShowOnesExtractDataType) &&
                    !OnesConstants.IgnoredOnesTypes.Contains(o.OnesTypeId)  &&
                    !OnesConstants.IgnoreOnesStatuses.Contains(o.StatusId) &&
                    onesFilter.BuIds.Contains(o.BuId) &&
                    onesFilter.SiteIds.Contains(o.SiteEntityId) &&
                    onesFilter.DisciplineIds.Contains(o.DisciplineId.Value))
                .Include(o => o.Artist)
                .Where(o => onesFilter.ArtistLevelIds.Contains(o.Artist.LevelId.Value))
                .ToList();

            return ones;
        }
    }
}
