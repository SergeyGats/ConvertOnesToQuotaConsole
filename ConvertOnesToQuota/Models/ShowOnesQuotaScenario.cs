using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvertOnesToQuota.Models
{
    [Table("ShowOnesQuotaScenario", Schema = "dwh")]
    public class ShowOnesQuotaScenario
    {
        public ShowOnesQuotaScenario()
        {
            ShowOnesQuotaUniqueFilters = new List<ShowOnesQuotaUniqueFilter>();
        }

        [Key]
        public int Id { get; set; }

        public int ShowId { get; set; }
        public Show Show { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(25)]
        public string Name { get; set; }
        public DateTime CreatedDateTimeUtc { get; set; }
        public bool IsMaster { get; set; }

        public ICollection<ShowOnesQuotaUniqueFilter> ShowOnesQuotaUniqueFilters { get; set; }
    }
}
