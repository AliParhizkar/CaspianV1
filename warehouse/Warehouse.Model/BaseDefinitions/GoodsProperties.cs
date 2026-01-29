using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("GoodsProperties", Schema = "wh")]
    public class GoodsProperties
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام ویژگی")]
        public string Name { get; set; }

        [DisplayName("نوع ویژگی")]
        public PropertyType PropertyType { get; set; }

        [DisplayName("حداقل مقدار")]
        public decimal? MinimumValue { get; set; }

        [DisplayName("حداقل مقدار")]
        public decimal? MaximumValue { get; set; }

        [DisplayName("تعداد ارقام اعشار")]
        public int? NumberDigit { get; set; }

        [DisplayName("حداکثر تعداد حروف")]
        public int? MaxLength { get; set; }

        [DisplayName("طول ثابت")]
        public bool? FixLength { get; set; }

        [DisplayName("طول")]
        public int? Length { get; set; }

        [CheckOnDelete("ویزگی دارای ایتم های لیست می باشد و امکان حذف آن وجود ندارد.")]
        public IList<PropertyList> Properties { get; set; }
    }
}
