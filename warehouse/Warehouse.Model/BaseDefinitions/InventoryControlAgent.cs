using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("InventoryControlAgents", Schema = "wh")]
    public class InventoryControlAgent
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نوع")]
        public int GoodsPropertiesId { get; set; }

        [ForeignKey(nameof(GoodsPropertiesId))]
        public GoodsProperties GoodsProperties { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("وضعیت")]
        public ActiveStatus ActiveStatus { get; set; }

        [DisplayName("موجودی ترکیبی")]
        public bool CombinedInventory { get; set; }
    }
}
