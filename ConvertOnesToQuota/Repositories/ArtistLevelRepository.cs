using System.Collections.Generic;
using System.Linq;
using ConvertOnesToQuota.Common;
using ConvertOnesToQuota.Models;

namespace ConvertOnesToQuota.Repositories
{
    public class ArtistLevelRepository : BaseRepository<ArtistLevel>
    {
        private readonly List<string> _quotaArtistLevels = new List<string>
        {
            "Artist",
            "Key Artist",
            "Lead"
        };

        public ArtistLevelRepository(ApplicationDatabaseContext context)
            : base(context)
        {

        }

        public List<ArtistLevel> GetArtistLevels()
        {
            var artistLevels = GetCollectionAsQueryable(l => _quotaArtistLevels.Contains(l.CareerLevel)).ToList();

            return artistLevels;
        }
    }
}
