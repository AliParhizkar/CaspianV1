using Caspian.Common;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Employment.Model
{
    [Table("Provinces", Schema = "emp")]
    public class Province
    {
        [Key]
        public int Id { get; set; }

        [DisplayName("عنوان")]
        public string Title { get; set; }

        [CheckOnDelete("استان دارای شهر می باشد و امکان حذف آن وجود ندارد")]
        public virtual IList<City> Cities { get; set; }
    }
}
