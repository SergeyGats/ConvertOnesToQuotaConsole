using System.Collections.Generic;
using System.Linq;
using ConvertOnesToQuota.Common;
using ConvertOnesToQuota.Models;

namespace ConvertOnesToQuota.Repositories
{
    public class EntityTpsRelationRepository : BaseRepository<EntityTpsRelation>
    {
        public EntityTpsRelationRepository(ApplicationDatabaseContext context) : base(context)
        {

        }

        public IEnumerable<EntityTpsRelation> GetEntityTpsRelations(int entityId)
        {
            var entityTpsRelations = GetCollectionAsQueryable(x => x.EntityId == entityId).ToList();
            return entityTpsRelations;
        }
    }
}
