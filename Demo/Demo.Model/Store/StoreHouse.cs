using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Warehouses", Schema = "demo")]
    public class Warehouse
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public ActiveType ActiveType { get; set; }

        /// <summary>
        /// رسیدهای انبار
        /// </summary>
        [CheckOnDelete("برای انبار رسید صادر شده و امکان حذف آن وجود ندارد")]
        public virtual ICollection<WarehouseReceipt> WarehouseReceipt { get; set; }
    }
}
