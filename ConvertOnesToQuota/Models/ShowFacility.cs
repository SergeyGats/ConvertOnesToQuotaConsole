using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ConvertOnesToQuota.Models
{
    [Table("ShowFacilities", Schema = "dwh")]
    public class ShowFacility
    {
        [Key]
        public int Id { get; set; }

        public int ShowId { get; set; }

        [ForeignKey("ShowId")]
        public Show Show { get; set; }

        public int EntityId { get; set; }

        public int FacilityBuId => GetBuId(EntityId);

        private int GetBuId(int entityId)
        {
            int buId;

            if (EntityId > 0 && EntityId < 1000)
            {
                buId = 2;
            }
            else if (EntityId > 1000 && EntityId < 2000)
            {
                buId = 1001;
            }
            else if (EntityId > 2000 && EntityId < 3000)
            {
                buId = 2001;
            }
            else if (EntityId > 3000 && EntityId < 4000)
            {
                buId = 3001;
            }
            else if (EntityId > 4000 && EntityId < 5000)
            {
                buId = 4001;
            }
            else if (EntityId > 5000 && EntityId < 6000)
            {
                buId = 5001;
            }
            else if (EntityId > 6000 && EntityId < 7000)
            {
                buId = 6001;
            }
            else if (EntityId > 7000 && EntityId < 8000)
            {
                buId = 7001;
            }
            else if (EntityId > 8000 && EntityId < 9000)
            {
                buId = 8001;
            }
            else if (EntityId > 9000 && EntityId < 10000)
            {
                buId = 9001;
            }
            else
            {
                throw new Exception($"Incorrect entity id: {EntityId}");
            }

            return buId;
        }
    }
}
