using System;
using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    /// <summary>
    /// رسید انبار
    /// </summary>
    [Table("WarehouseReceipt", Schema = "demo")]
    public class WarehouseReceipt
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// تاریخ حواله
        /// </summary>
        public DateTime? Date { get; set; }

        public int WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public virtual Warehouse Warehouse { get; set; }

        public string Comment { get; set; }

        [CheckOnDelete("برای حواله کالا ثبت شده وامکان حذف آن وجود ندارد")]
        public virtual IList<MaterialReceipt> MaterialReceipts { get; set; }
    }
}
