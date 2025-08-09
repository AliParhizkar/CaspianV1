using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Warehouse.Model
{
    [Table("SellerCategories", Schema = "wh")]
    public class SellerCategory
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("حوزه ی بالایی")]
        public int? CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public SellerCategory Category { get; set; }

        [CheckOnInsert("گروهبندی دارای زیرمجموعه می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<SellerCategory> Categories { get; set; }

        [CheckOnDelete("گروهبندی به فروشگاه اختصاص داده شده و امکان حذف آن وجود ندارد")]
        public ICollection<SellerCategoryMembership> SellerCategoryMemberships { get; set; }
    }
}
