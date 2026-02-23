using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("ProductClasses", Schema = "wh")]
    public class ProductClass
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("روش قیمت گذاری")]
        public PricingMethod PricingMethod { get; set; }

        [CheckOnDelete("از این طبقه کالا بعنوان در تعریف کالا استفاده شده است و امکان حذف آن وجود ندارد")]
        public ICollection<Goods> Goods { get; set; }
    }


}
