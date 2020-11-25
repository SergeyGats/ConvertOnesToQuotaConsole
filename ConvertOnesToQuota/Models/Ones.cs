using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvertOnesToQuota.Models
{
    [Table("Ones", Schema = "dwh")]
    public class Ones
    {
        [Key]
        public int OneId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Period { get; set; }

        public int SiteEntityId { get; set; }
        
        public int? DisciplineId { get; set; }

        public int ArtistId { get; set; }
        public Artist Artist { get; set; }

        public int ShowId { get; set; }

        [Column("Ones")]
        public double? Value { get; set; }

        [Column("Scenario")]
        public int ScenarioId { get; set; }

        public int StatusId { get; set; }

        public int BuId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DayOfPeriod { get; set; }

        public string ExtractDataType { get; set; }

        [Column("OT")]
        public double? OverTime { get; set; }

        public string OnesTypeId { get; set; }

        [NotMapped]
        public bool IsEpisodic => DayOfPeriod.HasValue;
    }
}
