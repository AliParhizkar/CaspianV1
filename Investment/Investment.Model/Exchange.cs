using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Investment.Model
{
    [Table("Exchanges", Schema = "ivm")]
    public class Exchange
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام ارز")]
        public string Name { get; set; }

        [MaxLength(200), DisplayName("شرح")]
        public string Description { get; set; }

        [CheckOnDelete("ارز دارای نرخ می باشد و امکان حذف آن وجود ندارد")]
        public ICollection<ExchangeRate> ExchangeRates { get; set; }
    }
}
