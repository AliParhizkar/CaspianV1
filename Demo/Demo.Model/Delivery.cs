using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Model
{
    [Table("Deliveries", Schema = "demo")]
    public class Delivery
    {
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// نام
        /// </summary>
        [DisplayName("نام")]
        public string FName { get; set; }

        /// <summary>
        /// نام خانوادگی
        /// </summary>
        [DisplayName("نام خانوادگی")]
        public string LName { get; set; }

        /// <summary>
        /// کد پرسنلی
        /// </summary>
        [DisplayName("کد پرسنلی")]
        public string Code { get; set; }

        /// <summary>
        /// سفارشهای پیک
        /// </summary>
        [CheckOnDelete("پیک دارای سفارش می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<Order> Orders { get; set; }    
    }
}
