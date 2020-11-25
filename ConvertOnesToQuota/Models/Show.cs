using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvertOnesToQuota.Models
{
    [Table("Show", Schema = "dwh")]
    public class Show
    {
        public Show()
        {
            QuotaScenarios = new List<ShowOnesQuotaScenario>();
        }

        public int ShowId { get; set; }

        [Required]
        [StringLength(255)]
        public string ShowCode { get; set; }

        public int? ProducerUserId { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsShow { get; set; }
        public int BuId { get; set; }
        public int ShowCategoryId { get; set; }

        public ICollection<ShowOnesQuotaScenario> QuotaScenarios { get; set; }
    }
}
