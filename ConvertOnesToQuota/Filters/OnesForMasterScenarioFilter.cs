using System.Collections.Generic;

namespace ConvertOnesToQuota.Filters
{
    public class OnesForMasterScenarioFilter
    {
        public int ShowOnesScenarioId { get; set; }
        public int ShowId { get; set; }
        public List<int> BuIds { get; set; }
        public List<int> ArtistLevelIds { get; set; }
        public List<int> SiteIds { get; set; }
        public List<int> DisciplineIds { get; set; }
    }
}
