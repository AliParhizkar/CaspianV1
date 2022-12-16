using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [Table("Cities", Schema = "emp")]
    public class City
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("استان")]
        public int ProvinceId { get; set; }

        [ForeignKey(nameof(ProvinceId))]
        public virtual Province Province { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [CheckOnDelete("این شهر محل تولد فرزندان می باشد و امکان حذف آن وجود ندارد")]
        public virtual ICollection<ChildrenProperties> ChildrenProperties { get; set; }
    }
}
