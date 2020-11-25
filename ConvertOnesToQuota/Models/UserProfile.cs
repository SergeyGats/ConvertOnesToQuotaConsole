using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvertOnesToQuota.Models
{
    [Table("UserProfile", Schema = "dbo")]
    public class UserProfile
    {
        [Key]
        public int UserId { get; set; }
        public bool Active { get; set; }
        public bool Deleted { get; set; }

        [Column("UserTypeID")]
        public int UserTypeId { get; set; }

        public int BuId { get; set; }
    }
}
