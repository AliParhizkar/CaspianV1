using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("Products", Schema = "mrk")]
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("کد")]
        public string Code { get; set; }

        [DisplayName("قیمت")]
        public int Price { get; set; }

        [DisplayName("تخفیف")]
        public int Discount { get; set; }

        [DisplayName("گروه محصول")]
        public int CategoryId { get; set; }

        [ForeignKey(nameof(CategoryId))]
        public ProductCategory Category { get; set; }

        [DisplayName("توضیحات"), MaxLength(255)]
        public string Description { get; set; }

        [CheckOnDelete("محصول داری توضیحات می باشد و امکان حذف آن وجود ندارد.")]
        public ICollection<ProductDescription> ProductDescriptions { get; set; }

        [CheckOnDelete("این محصول سفارش داده شده و امکان حذف آن وجود ندارد")]
        public ICollection<OrderDetail> OrderDetails { get; set; }

        [CheckOnDelete("محصول دارای تاپینگ می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<ProductTopping> ProductToppings { get; set; }

        [CheckOnDelete("محصول دارای پرینتر می باشد و امکان حذف آن وجود ندارد.")]
        public ICollection<PrinterProduct> PrinterProducts { get; set; }
    }
}
