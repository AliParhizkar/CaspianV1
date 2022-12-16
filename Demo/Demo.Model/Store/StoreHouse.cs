using Caspian.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("StoreHouses")]
    public class StoreHouse
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public ActiveType ActiveType { get; set; }

        /// <summary>
        /// رسیدهای انبار
        /// </summary>
        [CheckOnDelete("برای انبار رسید صادر شده و امکان حذف آن وجود ندارد")]
        public virtual ICollection<StoreHouseReceipt> StoreHouseReceipt { get; set; }
    }
}
