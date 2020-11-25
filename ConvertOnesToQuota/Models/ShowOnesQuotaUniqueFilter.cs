using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvertOnesToQuota.Models
{
    [Table("ShowOnesQuotaUniqueFilter", Schema = "dwh")]
    public class ShowOnesQuotaUniqueFilter
    {
        public ShowOnesQuotaUniqueFilter()
        {
            ShowOnesQuotas = new List<ShowOnesQuota>();
        }

        [Key]
        public int Id { get; set; }

        public int SiteId { get; set; }
        public int DisciplineId { get; set; }

        [Column("ScenarioId")]
        public int ShowOnesQuotaScenarioId { get; set; }
        public ShowOnesQuotaScenario ShowOnesQuotaScenario { get; set; }

        public double TotalShots { get; set; }
        public string Note { get; set; }

        public ICollection<ShowOnesQuota> ShowOnesQuotas { get; set; }
    }
}
