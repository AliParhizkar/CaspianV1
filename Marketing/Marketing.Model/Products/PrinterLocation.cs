using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Marketing.Model
{
    [Table("PrinterLocations", Schema = "mrk")]
    public class PrinterLocation
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("نام")]
        public string Name { get; set; }

        [DisplayName("وضعیت")]
        public ActiveType ActiveType { get; set; }

        [CheckOnDelete("پرینتر به محصول تخصیص داده شده و امکان حذف آن وجود ندارد")]
        public ICollection<PrinterProduct> PrinterProducts { get; set; }
    }
}
