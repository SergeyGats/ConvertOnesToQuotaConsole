using ConvertOnesToQuota.Common;
using ConvertOnesToQuota.Models;

namespace ConvertOnesToQuota.Repositories
{
    public class UserProfileRepository : BaseRepository<UserProfile>
    {
        public UserProfileRepository(ApplicationDatabaseContext context)
            : base(context)
        {

        }
    }
}
