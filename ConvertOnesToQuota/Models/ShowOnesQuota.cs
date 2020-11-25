using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvertOnesToQuota.Models
{
    [Table("ShowOnesQuota", Schema = "dwh")]
    public class ShowOnesQuota
    {
        [Key]
        public int Id { get; set; }

        [Column("UniqueFilterId")]
        public int ShowOnesQuotaUniqueFilterId { get; set; }
        public ShowOnesQuotaUniqueFilter ShowOnesQuotaUniqueFilter { get; set; }

        public int SeniorityLevelId { get; set; }
        public int MetricTypeId { get; set; }

        public decimal Value { get; set; }
        public DateTime Date { get; set; }
    }
}
