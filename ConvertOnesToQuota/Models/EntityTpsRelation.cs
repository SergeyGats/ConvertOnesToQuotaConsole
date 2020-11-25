using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvertOnesToQuota.Models
{
    [Table("EntityTPSRelations", Schema = "dbo")]
    public class EntityTpsRelation
    {
        [Key, Column(Order = 1)]
        public int EntityId { get; set; }

        [Key]
        [Column("TPSEntityID", Order = 2)]
        public int TpsEntityId { get; set; }

        public DateTime ValidFrom { get; set; }
        public DateTime ValidTo { get; set; }
    }
}
