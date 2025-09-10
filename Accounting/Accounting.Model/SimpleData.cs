using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Accounting.Model
{
    [Table("SimpleData", Schema = "acc")]
    public class SimpleData
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [DisplayName("نوع")]
        public SimpleDataType DataType { get; set; }
    }
}
