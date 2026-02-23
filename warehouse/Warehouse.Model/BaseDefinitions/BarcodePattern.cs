using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("BarcodePatterns", Schema = "wh")]
    public class BarcodePattern
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("کد الگو")]
        public string Code { get; set; }

        [DisplayName("نام الگو")]
        public string Name { get; set; }

        [CheckOnDelete("از این الگوی کد در کدینگ کالا استفاده شده است و امکان حذف آن وجود ندارد")]
        public ICollection<Goods> Goods { get; set; }
    }
}
