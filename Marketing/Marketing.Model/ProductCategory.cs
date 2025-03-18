using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("ProductCategories", Schema = "mrk")]
    public class ProductCategory
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("گروه محصول")]
        public int? CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public ProductCategory Category { get; set; }

        [DisplayName("وضعیت")]
        public ActiveType ActiveType { get; set; }

        [CheckOnDelete("گروه دارای محصول می باشد و امکان حذف آن وجود ندارد")]
        public IList<Product> Products { get; set; }

        [CheckOnDelete("گروه دارای زیر گروه می باشد و امکان حذف آن وجود ندارد.")]
        public IList<ProductCategory> Categories { get; set; }

        [DisplayName("توضیحات"), MaxLength(255)]
        public string Description { get; set; }
    }
}
