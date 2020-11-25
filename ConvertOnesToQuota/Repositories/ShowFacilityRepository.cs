using System.Collections.Generic;
using System.Linq;
using ConvertOnesToQuota.Common;
using ConvertOnesToQuota.Models;

namespace ConvertOnesToQuota.Repositories
{
    public class ShowFacilityRepository : BaseRepository<ShowFacility>
    {
        public ShowFacilityRepository(ApplicationDatabaseContext context) : base(context)
        {
        }

        public IEnumerable<ShowFacility> GetShowFacilities(int showId)
        {
            var showFacilities = GetCollectionAsQueryable(sf => sf.ShowId == showId)
                .ToList();

            return showFacilities;
        }
    }
}
