using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("PropertiesList", Schema = "wh")]
    public class PropertyList
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        public int GoodsPropertyId { get; set; }

        [ForeignKey(nameof(GoodsPropertyId))]
        public GoodsProperties GoodsProperty { get; set; }

        [CheckOnDelete("این آیتم بعنوان ویژگی تعریف شده و امکان حذف آن وجود ندارد")]
        public ICollection<PropertyOfGoods> PropertyOfGoods { get; set; }
    }
}
