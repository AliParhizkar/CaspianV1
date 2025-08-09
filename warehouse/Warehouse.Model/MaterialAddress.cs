using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("MaterialAddresses", Schema = "wh")]
    public class MaterialAddress
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("انبار")]
        public int StockroomId { get; set; }

        [ForeignKey(nameof(StockroomId))]
        public StockRoom StockRoom { get; set; }

        public int? ParentAddressId { get; set; }

        [ForeignKey(nameof(ParentAddressId))]
        public MaterialAddress ParentAddress { get; set; }

        [CheckOnDelete("آدرس دارای آدرس فرعی می باشد و امکان حذف آن وجود ندارد.")]
        public ICollection<MaterialAddress> MaterialAddresses { get; set; }
    }
}
