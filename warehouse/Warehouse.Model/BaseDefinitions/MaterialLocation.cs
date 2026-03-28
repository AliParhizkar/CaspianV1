using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("MaterialLocations", Schema = "wh")]
    public class MaterialLocation
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("انبار")]
        public int StockroomId { get; set; }

        [ForeignKey(nameof(StockroomId))]
        public StockRoom StockRoom { get; set; }

        public int? ParentLocationId { get; set; }

        [ForeignKey(nameof(ParentLocationId))]
        public MaterialLocation ParentLocation { get; set; }

        [CheckOnDelete("محل فیزیکی انبار دارای زیرمجموعه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<MaterialLocation> Locations { get; set; }

        [CheckOnDelete("کالا(هایی) در این محل فیزیکی ذخیره شده اند و امکان حذف آن وجود ندارد")]
        public ICollection<GoodsPlacement> GoodsPlacements { get; set; }
    }
}
