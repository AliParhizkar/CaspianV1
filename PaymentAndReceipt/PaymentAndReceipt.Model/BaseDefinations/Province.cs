using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PaymentAndReceipt.Model
{
    [Table("provinces", Schema = "par")]
    public class Province
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Name { get; set; }

        [CheckOnDelete("استان دارای شهر می باشد و امکان حذف آن وجود ندارد")]
        public IList<City> Cities { get; set; }
    }
}
