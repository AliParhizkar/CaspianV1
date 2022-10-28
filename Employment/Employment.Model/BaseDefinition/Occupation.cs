using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    /// <summary>
    /// رسته شغلی
    /// </summary>
    [Table("Occupations", Schema = "emp")]
    public class Occupation
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [CheckOnDelete("رسته شغلی دارای رشته شعلی می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<CareerField> CareerFields { get; set; }
    }
}
