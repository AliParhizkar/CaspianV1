using Caspian.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

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

        public DateTime? Date { get; set; }

        [DisplayName("Warehouse")]
        public int WarehouseId { get; set; }

        [ForeignKey(nameof(WarehouseId))]
        public virtual Warehouse Warehouse { get; set; }

        public string Comment { get; set; }

        [CheckOnDelete("The warehouse receipt contains the goods and cannot be deleted.")]
        public virtual IList<MaterialReceipt> MaterialReceipts { get; set; }
    }
}
