using System.Data.Entity;
using System.Linq;
using ConvertOnesToQuota.Common;
using ConvertOnesToQuota.Models;

namespace ConvertOnesToQuota.Repositories
{
    public class ShowOnesQuotaScenarioRepository : BaseRepository<ShowOnesQuotaScenario>
    {
        public ShowOnesQuotaScenarioRepository(ApplicationDatabaseContext context)
            : base(context)
        {

        }

        public int Save(ShowOnesQuotaScenario quotaScenario)
        {
            Insert(quotaScenario);
            Context.SaveChanges();

            return quotaScenario.Id;
        }

        public new int AddOrUpdate(ShowOnesQuotaScenario quotaScenario)
        {
            base.AddOrUpdate(quotaScenario);
            return quotaScenario.Id;
        }

        public void DeleteQuotaScenariosForBusinessUnit(int buId)
        {
            var scenarios = GetCollectionAsQueryable(s => s.Show.BuId == buId)
                .Include(s => s.Show).ToList();

            Delete(scenarios);
            Context.SaveChanges();
        }
    }
}
