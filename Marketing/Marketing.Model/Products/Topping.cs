using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("Toppings", Schema = "mrk")]
    public class Topping
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("قیمت")]
        public int Price { get; set; }

        [CheckOnDelete("این تاپینگ به محصول اختصاص داده شده و امکان حذف آن وجود ندارد")]
        public ICollection<ProductTopping> ProductToppings { get; set; }

        [CheckOnDelete("این تاپینگ سفارش داده شده و امکان حذف آن وجود ندارد")]
        public ICollection<OrderDetailTopping> OrderDetailToppings { get; set; }
    }
}
