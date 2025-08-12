using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("MaterialUnits", Schema = "wh")]
    public class MaterialUnit
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("مقیاس")]
        public MeasureType MeasureType { get; set; }

        [MaxLength(200),DisplayName("شرح")]
        public string Description { get; set; }

        [DisplayName("کالایی با این مقیاس تعریف شده است و امکان حذف آن وجود ندارد")]
        public ICollection<Product> Products { get; set; }
    }
}
