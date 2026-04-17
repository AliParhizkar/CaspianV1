using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("Configs", Schema = "mrk")]
    public class Config
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("پذیرنده")]
        public string Name { get; set; }

        [DisplayName("مشتری می تواند چندین آدرس داشته باشد")]
        public bool CustomerHasManyAddresses { get; set; }

        [DisplayName("مشتری عضو چندین گروه است")]
        public bool CustomerIsMemberOfGroups { get; set; }

        [DisplayName("روش مدیریت آدرس پیشفرض")]
        public DefaultAddressManagement DefaultAddressManagement { get; set; }

        [DisplayName("زمان بازگشایی")]
        public TimeOnly OpenTime { get; set; }

        /// <summary>
        /// Category Display type in product page
        /// </summary>
        [DisplayName("نوع نمایش گروه محصول")]
        public CategoryType CategoryType { get; set; }

        [DisplayName("نوع تخفیف")]
        public DiscountType DiscountType { get; set; }

        [DisplayName("درصد تخفیف")]
        public decimal PercentDiscount { get; set; }

        [DisplayName("روش رند کردن")]
        public RoundType? RoundType { get; set; }

        [DisplayName("مبلغ رند")]
        public int? RoundAmount { get; set; }

        [DisplayName("نوع سفارش پیشفرض")]
        public OrderType DefaultOrderType { get; set; }

        [DisplayName("تاخیر زمانی")]
        public int DelayMilliSecond { get; set; }
    }
}
