using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvertOnesToQuota.Models
{
    [Table("Artist", Schema = "dwh")]
    public class Artist
    {
        public Artist()
        {
            Ones = new List<Ones>();
        }

        [Key]
        public int ArtistId { get; set; }
        
        public int? LevelId { get; set; }

        public ICollection<Ones> Ones { get; set; }
    }
}
