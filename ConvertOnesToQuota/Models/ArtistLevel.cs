using System.ComponentModel.DataAnnotations.Schema;

namespace ConvertOnesToQuota.Models
{
    [Table("ArtistLevel", Schema = "ref")]
    public class ArtistLevel
    {
        public int Id { get; set; }
        public string CareerLevel { get; set; }
        public int BuId { get; set; }
    }
}
