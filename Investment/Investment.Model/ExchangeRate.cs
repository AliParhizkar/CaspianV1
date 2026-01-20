using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("ExchangeRates", Schema = "ivm")]
    public class ExchangeRate
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("ارز")]
        public int ExchangeId { get; set; }

        [ForeignKey(nameof(ExchangeId))]
        public Exchange Exchange { get; set; }

        [DisplayName("تاریخ موثر")]
        public DateOnly Date { get; set; }

        [DisplayName("نرخ")]
        public int Rate { get; set; }
    }
}
